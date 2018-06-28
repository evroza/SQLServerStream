using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLServerNotifyStream.Framework;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SQLServerNotifyStream
{
    // This class handles all transmission operations to the listening http server - with fail checks and support for local logging
    class RecordHandler
    {
        // Handles login to webserver - returns valid token for use in further transactions - called on service start to renew token
        public static async Task<string> LoginAsync(string webServerAddress, string username, string password)
        {
            string token = "";
            var values = new Dictionary<string, string>
            {
               { "username", username },
               { "password", password }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await Globals.Client.PostAsync( webServerAddress + "/login", content);

            var responseString = await response.Content.ReadAsStringAsync();

            LoginResponseJSON payload = JsonConvert.DeserializeObject<LoginResponseJSON>(responseString);

            if (payload.status.Equals("success"))
            {
                // Login was successful, update global token
                Globals.Token = payload.token;
                token = Globals.Token;

            } else if (payload.status.Equals("error"))
            {
                // The login details likely incorrect or some other error - login was unsuccessful
                // First set the TokenInvalid flag to true
                // Trigger a console display on screen with error message that credentials are invalid
                Globals.InvalidateToken();
                token = "";

                ConsoleHarness.WriteToConsole(ConsoleColor.Red, "ERROR: Problem logging in to Web Server, Credentials likely invalid. Username: {0}, Password: {1}", Globals.WebServerUsername, Globals.WebServerPassword);
            }

            return token;
        }

        public static async Task<string> RenewTokenAsync()
        {
            // Triggers a login call
            await LoginAsync(Globals.WebServerAddress, Globals.WebServerUsername, Globals.WebServerPassword);

            return Globals.Token;
        }

        public static async Task<bool> TransmitAsync(RecordChangedEventArgs<ModelOrderProducts> e)
        {
            // return true on successful transmission of a record, false otherwise

            var changedEntity = e.Entity;

            dynamic record = new JObject();
            record.dataset = new JObject();
            record.dataset.ItemID = changedEntity.itemid;
            record.dataset.CreatedAt = changedEntity.Created_at;
            record.dataset.QuantityOrdered = changedEntity.qty_ordered;
            record.dataset.Price = changedEntity.price;

            // First remove if already exists
            Globals.Client.DefaultRequestHeaders.Remove("Authorization");
            // Add authorization header
            Globals.Client.DefaultRequestHeaders.Add("Authorization", Globals.Token);
            StringContent stringContent = new StringContent(record.ToString(),
                        UnicodeEncoding.UTF8,
                        "application/json");
            var response = await Globals.Client.PostAsync(Globals.WebServerAddress + "/save", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();

            GenericResponseJSON payload = JsonConvert.DeserializeObject<GenericResponseJSON>(responseString);

            if (payload.status.Equals("success"))
            {
                // Mark this record as transmitted in db
                MarkTransmitted("" + changedEntity.itemid);

                return true;
            } else
            {
                // If function still executing then transmit was unsuccesful, log it locally for later transmission
                LogFailed(record.ToString());

                // If not successful transmit the return false
                return false;
            }            
        }

        private static void MarkTransmitted(string recordID)
        {
            // Performs query on SQL Server marking the current record as trasmitted
            // Called as soon as a success message is received from web server after calling the /save api endpoint
            using (SqlConnection connection = new SqlConnection(Globals.DBConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE order_products SET Created_at = @cra, qty_ordered = @qty WHERE itemid = @id";


                //command.Parameters.AddWithValue("@tbl", Globals.DBTableName);
                command.Parameters.AddWithValue("@id", recordID); // Mess with this figure to see changes on different records
                command.Parameters.AddWithValue("@cra", "2013-12-03 00:00:00.000");
                command.Parameters.AddWithValue("@qty", "11.1111");

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

        }
        
        public static void LogFailed(string record)
        {
            // This function logs failed transmissions to local file system. They will be retrasmitted later when token is valid again
            String timeStamp = DateTime.Now.ToString();
            string fileName = timeStamp + ".txt";
            string path = AppDomain.CurrentDomain.BaseDirectory + fileName;

            try
            {
                
                
                // Next write the dataset to the just created file
                if (!File.Exists(path))
                {
                    // try creating a new file on local system and write to it the dataset that failed to transmit
                    // file name should be the current timestamp
                    System.IO.File.Create(path);

                    // File was created. Append dataset to it
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(record);
                    }
                } else
                {
                    // File wasn't created or has been deleted throw error
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public static void RetransmitFailed(string data)
        {
            // Whenever the Web server token is updated, this function is called to check whether there are any 
            // failed trasmissions and retries trasmitting them again - on success, the local logged file is deleted

        }
    }
}

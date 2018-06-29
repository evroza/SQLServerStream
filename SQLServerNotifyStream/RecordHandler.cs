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

            bool transmitStatus =  await TransmitJSONRecordAsync(record.ToString());
            

            return transmitStatus;            
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
            Int32 timeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = $"{timeStamp}.txt";
            string logFolder = Globals.LogFolder;
            string path = AppDomain.CurrentDomain.BaseDirectory + logFolder + "/" + fileName;

            try
            {          
                
                // Next write the dataset to the just created file
                if (!File.Exists(path))
                {
                    // try creating a new file on local system and write to it the dataset that failed to transmit
                    // file name should be the current timestamp

                    using (StreamWriter sw = File.CreateText(path))
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

                Console.WriteLine(e.Message);
            }
        }

        private static void TransmitAsyncRecursionProxyAsync()
        {
            // retries retrasmitting n times, if recursed two times token still invalid, probable poisoning attack, abort and log to file instead

        }

        public static async Task<bool> TransmitJSONRecordAsync(string record)
        {
            // Takes a JSON formatted record and transmits it to web server /save endpoint
            // ANY failed transmits are logged to file system and the fuction return false after that
            // Successful trasmits return true


            // First remove if already exists
            Globals.Client.DefaultRequestHeaders.Remove("Authorization");
            // Add authorization header
            Globals.Client.DefaultRequestHeaders.Add("Authorization", Globals.Token);
            StringContent stringContent = new StringContent(record,
                        UnicodeEncoding.UTF8,
                        "application/json");
            var response = await Globals.Client.PostAsync(Globals.WebServerAddress + "/save", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();

            GenericResponseJSON payload = JsonConvert.DeserializeObject<GenericResponseJSON>(responseString);
            // Deserialize the original 'record' JSON sting to object so we can extract the itemid
            GenericRecordJSON changedEntity = JsonConvert.DeserializeObject<GenericRecordJSON>(record);


            if (payload.status.Equals("success"))
            {
                // Mark this record as transmitted in db
                MarkTransmitted("" + changedEntity.dataset.itemid);

                return true;
            }
            else
            {
                // First check the error message, if it is because the token is invalid, renew it and resend, otherwise, log it to file
                if (payload.message.Equals("Invalid token submitted!"))
                {
                    // First trigger a token refresh by logging in again
                    await RenewTokenAsync();
                    // Then try retransmit - recursive call
                    // TODO: Later can add counter for retrasmit of current record, if twice then don't redo recursive renewal of token, just log to file
                    //TransmitAsyncRecursionProxyAsync();

                    //await TransmitAsync(e);
                    // After transmit return true
                }

                // If function still executing then transmit was unsuccesful, log it locally for later transmission
                LogFailed(record.ToString());

                // If not successful transmit the return false
                return false;
            }
    
        }

            public static async Task<bool> RetransmitFailedLocalAsync()
        {
            // Whenever the Web server token is updated, this function is called to check whether there are any 
            // failed trasmissions and retries trasmitting them again - on success, the local logged file is deleted
            // Checks the logging folder and loops through each file one by one retransmitting it and on success, deleting it
            bool transmitAllSucceed = true;
            string logFolder = Globals.LogFolder;
            string directory = AppDomain.CurrentDomain.BaseDirectory + logFolder;

            string[] files = System.IO.Directory.GetFiles(directory);
            if (files.Length == 0)
            {
                Console.WriteLine($"Empty: There are no locally logged Datasets, {directory} directory is clean!");

                return transmitAllSucceed;
            }
            else
            {
                Console.WriteLine($"Found {files.Length} Locally logged Datasets in Directory: {directory}, Initating retransmit");

                // Loop through each file in the 'files' array, trasmitting it and deleting it
                foreach (string fi in files)
                {
                    Console.WriteLine($"Retransmitting dataset in File: {fi}");
                    bool currFileSuccessTransmit = false;

                    using (StreamReader file = File.OpenText(fi))
                    {
                        // Get the whole record from file
                        // TODO: Add try catch - and also verify document structure before attempt to transmit
                        string record = await file.ReadToEndAsync();

                        //retransmit it - if successful, delete this file, if not move to next file
                        bool success = await TransmitJSONRecordAsync(record);

                        if (!success)
                        {
                            // toggle flag, not all local records were transmitted successfully
                            transmitAllSucceed = false;
                        }
                        else
                        {
                            // Current file successfully transmitted, toggle flag to true
                            currFileSuccessTransmit = true;
                        }

                    }

                    // Current file has already been closed, check if it was succesfully transmitted then delete
                    // Otherwise do nothing
                    // This record was successfully transmitted, delete it from local file system
                    if (File.Exists(fi) && currFileSuccessTransmit)
                    {
                        Console.WriteLine($"DELETE LOCAL: Successful retransmision of File: {fi}");
                        File.Delete(fi);
                    }

                }
            }

            return transmitAllSucceed;
        }

        public static async Task<bool> RetransmitFailedDBAsync()
        {
            // This method performs a manual check (query) on DB to find records that have not been marked as trasmitted and transmit them
            // Can ignore the last 10 records for safety, this will prevent race condition where we manually retransmit a record that is currently
            // being processed, i.e it just fired it's insert listener and is still being processed then we end up manually retransmitting before 
            // it is finished processing

            // Performs query on SQL Server marking the current record as trasmitted
            // Called as soon as a success message is received from web server after calling the /save api endpoint

            // If even one record fails to transmit and is instead logged to file, this  flag set to false, use it to initiate local files retransmition
            bool transmitStatusAll = true;

            using (SqlConnection connection = new SqlConnection(Globals.DBConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM order_products WHERE price = @prc";


                //command.Parameters.AddWithValue("@tbl", Globals.DBTableName);
                command.Parameters.AddWithValue("@prc", "2.8800"); // Mess with this figure to see changes on different records

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic record = new JObject();
                        record.dataset = new JObject();
                        record.dataset.ItemID = reader["itemid"].ToString();
                        record.dataset.CreatedAt = reader["Created_at"].ToString();
                        record.dataset.QuantityOrdered = reader["qty_ordered"].ToString();
                        record.dataset.Price = reader["price"].ToString();

                        try
                        {
                            // Transmit one by one to web server /save
                            bool transmitStatus = await TransmitJSONRecordAsync(record.ToString());

                            if (transmitStatus)
                            {
                                Console.WriteLine($"DBTOFILESERVER: Record ID:  {record.dataset.ItemID} successfully transmitted");
                            }
                            else
                            {
                                Console.WriteLine($"FAILED:DBTOFILESERVER:  Record {record.dataset.ItemID} transmition failed! It has been logged to file system for later transmittion");

                                transmitStatusAll = false;
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            Console.WriteLine("DBTOFILESERVER: A HTTP connection error was encountered, could be because of Web Server being down/unreachable");
                            Console.WriteLine($"FAILED:DBTOFILESERVER:  Record {record.dataset.ItemID} transmition failed! It has been logged to file system for later transmittion");

                            transmitStatusAll = false;
                            
                            // Log current record to File system, it was never trasmitted in the first place due to connection issues
                            RecordHandler.LogFailed(record.ToString());
                        }
                    }

                    // Each record trasmitted independently thus will have own status
                    // If current record transmitStatus is true, then transmitStatus remains true
                    // If current record transmitStatus is false then log record to file system instead

                }
                connection.Close();
            }                

            return transmitStatusAll;
        }


    }
}

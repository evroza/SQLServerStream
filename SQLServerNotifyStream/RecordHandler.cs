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

namespace SQLServerNotifyStream
{
    // This class handles all transmission operations to the listening http server - with fail checks and support for local logging
    class RecordHandler
    {
        // Handles login to webserver - returns valid token for use in further transactions - called on service start to renew token
        public static async Task<string> Login(string webServerAddress, string username, string password)
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

        public static async Task<string> RenewToken()
        {
            // Triggers a login call
            await Login(Globals.WebServerAddress, Globals.WebServerUsername, Globals.WebServerPassword);

            return Globals.Token;
        }

        public static async Task<bool> Transmit(RecordChangedEventArgs<ModelOrderProducts> e)
        {
            // return true on successful transmission of a record, false otherwise

            var changedEntity = e.Entity;
            var record = new Dictionary<string, string>
            {
               { "ItemID", "" + changedEntity.itemid },
               { "CreatedAt", changedEntity.Created_at },
               { "QuantityOrdered", "" + changedEntity.qty_ordered },
               { "Price", "" + changedEntity.price },
            };

            var content = new FormUrlEncodedContent(record);
            var response = await Globals.Client.PostAsync(Globals.WebServerAddress + "/save", content);
            var responseString = await response.Content.ReadAsStringAsync();

            GenericResponseJSON payload = JsonConvert.DeserializeObject<GenericResponseJSON>(responseString);

            return false;
        }
    }
}

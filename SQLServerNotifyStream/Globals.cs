using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;


namespace SQLServerNotifyStream
{
    
    static class Globals
    {
        public static readonly HttpClient Client = new HttpClient();
        // Holds the token on login, this is updated on every login call
        private static string _token = "";
        public static string Token {
            get {
                return _token;
            }
            set {
                // Update _token and also update TokenValid Flag
                TokenInvalid = false;
                _token = value;
            }
        }
        // If login to server unsuccessful, this flag is set to true which triggers local file system logging of records
        public static bool TokenInvalid { get; private set; } // can only be set inside class
        public static string WebServerAddress = FetchGlobalConfig("WebServerAddress");
        //Username used to login to the Web server
        public static readonly string WebServerUsername = FetchGlobalConfig("WebServerUsername");
        //password used to login to Web server
        public static readonly string WebServerPassword = FetchGlobalConfig("WebServerPassword");

        public static readonly string DBConnectionString = DBConnectionStringRead(FetchGlobalConfig("DBServerType")); // no need to use app config vars for now
        // For local debugging, swap with top statement for production
        //public static readonly string DBConnectionString = "data source=.; initial catalog=yanguTest; integrated security=True"; // no need to use app config vars for now
        public static readonly string DBTableName = FetchGlobalConfig("DBTableName");
        // For local debugging, swap with top statement for production
        //public static readonly string DBTableName = "yanguTest"; // 

        public static readonly string LogFolder = FetchGlobalConfig("LogFolder"); 
        public static readonly string CrashLogFolder = FetchGlobalConfig("CrashLogFolder"); 

        // Retransmit interval of failed transmits default was 10 minutes i.e 60*10*1000 (microseconds)
        public static readonly int FailedRetransmitInterval = Int32.Parse(FetchGlobalConfig("FailedRetransmitIntervalSeconds")) * 1000;


        public static bool IsTokenInvalid()
        {
            // returns status of flag
            return TokenInvalid;
        }
        public static void InvalidateToken()
        {
            // Sets the loginInvalid to true
            TokenInvalid = true;
        }

        public static string FetchGlobalConfig(string key)
        {
            // Fetches the configuration 'key' from the App.Config settings file

            string result;

            try
            {
                var globalConfigs = ConfigurationManager.AppSettings;
                result = globalConfigs[key] ?? "__Setting_Not_Found__";
                Console.WriteLine("CONFIG LOADED - key: " + result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                result = "__Setting_Not_Found__";
            }
            return result;
        }

        public static string DBConnectionStringRead(string serverType = "remote")
        {
            // Returns the appropriate SQL Server connection string based on whether the DB server is
            // Local or remote -- specified by the input parameter serverType
            string connectionString = "";

            if (serverType == "remote")
            {
                connectionString = ConfigurationManager.ConnectionStrings["DBServerRemote"].ConnectionString;
            } else if (serverType == "local")
            {
                connectionString = ConfigurationManager.ConnectionStrings["DBServerLocal"].ConnectionString;
            } else
            {
                connectionString = "__Setting_Not_Found__";
            }

            return connectionString;
        }




        }
}

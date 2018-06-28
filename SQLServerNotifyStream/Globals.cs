using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


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
        public static string WebServerAddress = "http://localhost:3000";
        //Username used to login to the web server
        public static string WebServerUsername { get; set; }
        //password used to login to webs server
        public static string WebServerPassword { get; set; }


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

    }
}

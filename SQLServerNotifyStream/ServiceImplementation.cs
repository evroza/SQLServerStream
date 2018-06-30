using SQLServerNotifyStream.Framework;
using System;
using System.IO;
using System.ServiceProcess;


namespace SQLServerNotifyStream
{
    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("SQLServerNotifyStream",
        DisplayName = "SQLServerNotifyStream",
        Description = "SQLServerNotifyStream service listens for DB table inserts and posts the changed record to a web API",
        EventLogSource = "SQLServerNotifyStream",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public async void OnStart(string[] args)
        {
            //For debug, create a file when service starts
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {

                // Create the 
                //First create the Globals.LogFolder and Globals.CrashLogFolder if it don't exist:
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Globals.CrashLogFolder); // Catch-all error log folder
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Globals.LogFolder); //Failed transmits folder

                // Login on every service start - to renew token if expired
                // MUST perform login before ANYTHING else, otherwise the login might not be called if other event listeners are invoked
                await SQLServerStream.LoginAsync(Globals.WebServerAddress, Globals.WebServerUsername, Globals.WebServerPassword);
                SQLServerStream.StartRetransmitIntervals();

                SQLServerStream listener = new SQLServerStream();
            }
            catch (Exception e)
            {

                // This function logs failed transmissions to local file system. They will be retrasmitted later when token is valid again
                Int32 timeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string fileName = $"{timeStamp}_UnhandledExeption.log";
                string logFolder = Globals.CrashLogFolder;
                string path = AppDomain.CurrentDomain.BaseDirectory + logFolder + "/" + fileName;

                try
                {
                    Console.WriteLine("Unhandlable error : " + e.Message);
                    
                    // Next write the dataset to the just created file
                    File.WriteAllText(path, "Unhandlable error : " + e.Message);
                    File.AppendAllText(path, e.StackTrace.ToString());
                    Environment.Exit(10);
                }
                catch (Exception err)
                {

                    Console.WriteLine(err.Message);

                    // Try writing the new thrown exeption to current base directory as last resort
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + fileName, "Error Logging unhandled error : " + e.Message);
                }
            }
            
            
                      
        }

        
        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
            //For debug, create a file when service stops
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStop.txt");
            // TODO: Can initiate a logout call, but not necessary
        }

        /// <summary>
        /// This method is called when a service gets a request to pause,
        /// but not stop completely.
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
        }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
        }

        /// <summary>
        /// This method is called when a custom command is issued to the service.
        /// </summary>
        /// <param name="command">The command identifier to execute.</param >
        public void OnCustomCommand(int command)
        {
        }
    }
}

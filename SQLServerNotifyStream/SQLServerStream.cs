﻿using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLServerNotifyStream.JSONConvert;
using System.Threading;

namespace SQLServerNotifyStream
{
    class SQLServerStream
    {

        private static System.Timers.Timer aTimer;
        private static int  counter = 1; // For debug to check whether the interval is running

        // The constructor initializes the database connections and adds column mappings if any
        public SQLServerStream()
        {
            /**
             * No need to do mappings since model class properties exactly match the db column names. If otherwise then need to
                var mapper = new ModelToTableMapper<Customer>();
                mapper.AddMapping(c => c.Surname, "Second Name");
                mapper.AddMapping(c => c.Name, "First Name");
            **/

            using (var dep = new SqlTableDependency<ModelProcessedHistory>(Globals.DBConnectionString, Globals.DBTableName))
            {
                dep.OnChanged += Changed;
                dep.Start();

                Console.WriteLine("Press a key to exit");
#if DEBUG
                // Need to use this in debug mode to read user input keys to either quit, pause or resume service
                // In other location, the program is kept alive by infite loop if key is not either if Q, P or R
                // In Progaram.Main(), ConsoleHarness.Run() is called for every input key which in turn determines whether 
                // the service should (P)ause, (R)esume or (Stop) OR keep running infinitely for any other key that isn't
                // in this set. Makes the service easy to debug
                Console.ReadKey();
#else
                // We don't really need  Console.ReadKey(); here since the console input isn't important
                // Just need to put the service in infinite loop waiting for user input thus keeping it alive
                Thread.Sleep(Timeout.Infinite);

#endif


                dep.Stop();
            }
        }

        public static async void Changed(object sender, RecordChangedEventArgs<ModelProcessedHistory> e)
        {
            var changedEntity = e.Entity;

            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine("Item ID: " + changedEntity.HistoryID);
            Console.WriteLine("ModelElementID: " + changedEntity.ModelElementID);
            Console.WriteLine("ModelJobID: " + changedEntity.ModelJobID);
            Console.WriteLine("OrderID: " + changedEntity.OrderID);
            //RecordHandler.RetransmitFailedLocalAsync(); // Debug only, delete it from here when done
            if (e.ChangeType == TableDependency.Enums.ChangeType.Insert) {
                //DML operation of type insert, try push this entry to logging server
                try {
                    bool transmitStatus = await RecordHandler.TransmitAsync(e);

                    if (transmitStatus)
                    {
                        Console.WriteLine($"Record ID:  {changedEntity.HistoryID} successfully transmitted");
                    } else
                    {
                        // This section executed if either the transmission failed and record was logged to file OR!
                        // The UnitsCounted field was had a value Zero causing a reconcillitation action to be performed
                        Console.WriteLine($"FAILED: Record {changedEntity.HistoryID} transmition failed! Deffered processing");
                    }

                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("A HTTP connection error was encountered, could be because of Web Server being down/unreachable");
                    Console.WriteLine($"FAILED: Record {changedEntity.HistoryID} transmition failed! It has been logged to file system for later transmittion");

                    await RecordHandler.RetransmitFailedDBAsync(); // This statement is only for debug must be DELETED when done debugging!

                    dynamic record = JSONConverter.EntityToJObject(e);

                    // If record is null then return, don't continue processing
                    if(record == null)
                    {
                        return;
                    }
                    


                    // Log current record to File system, it was never trasmitted in the first place due to connection issues
                    RecordHandler.LogFailed(record.ToString());
                }
            }
        }

        

        public static void StartRetransmitIntervals()
        {
            // Next can initiate the timer interval for retransmision of local and DB datasets
            aTimer = new System.Timers.Timer(Globals.FailedRetransmitInterval);
            aTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            aTimer.Enabled = true;
        }

        public static void TimerElapsed(object source, ElapsedEventArgs e)
        {
            // This method invokes the Local file retransmit function FIRST, THEN invokes the db retransmit method
            // Don't need to await this, it can happen any time after, I just need it to be executed at some point
            RecordHandler.RetransmitFailedAsync();

            Console.WriteLine($"Retransmit Interval invoked at: {e.SignalTime}; Invoked: ({counter}) times since Service start.");
            SQLServerStream.counter++;



        }



            public static async Task LoginAsync(string WebServerAddress, string WebServerUsername, string WebServerPassword)
        {
            
            await RecordHandler.LoginAsync(WebServerAddress, WebServerUsername, WebServerPassword);

        }
        
    }
}

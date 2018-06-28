using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using Newtonsoft.Json;

namespace SQLServerNotifyStream
{
    class SQLServerStream
    {
        
        // The constructor initializes the database connections and adds column mappings if any
        public SQLServerStream()
        {
            /**
             * No need to do mappings since model class properties exactly match the db column names. If otherwise then need to
                var mapper = new ModelToTableMapper<Customer>();
                mapper.AddMapping(c => c.Surname, "Second Name");
                mapper.AddMapping(c => c.Name, "First Name");
            **/

            using (var dep = new SqlTableDependency<ModelOrderProducts>(Globals.DBConnectionString, Globals.DBTableName))
            {
                dep.OnChanged += Changed;
                dep.Start();

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();

                dep.Stop();
            }
        }

        public static async void Changed(object sender, RecordChangedEventArgs<ModelOrderProducts> e)
        {
            var changedEntity = e.Entity;

            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine("Item ID: " + changedEntity.itemid);
            Console.WriteLine("Created at: " + changedEntity.Created_at);
            Console.WriteLine("Quantity Ordered: " + changedEntity.qty_ordered);
            Console.WriteLine("Price: " + changedEntity.price);

            if (e.ChangeType == TableDependency.Enums.ChangeType.Insert) {
                //DML operation of type insert, push this entry to logging server
                await RecordHandler.TransmitAsync(e);
                try {

                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("A HTTP connection error was encountered, record will be logged to file ...");
                    RecordHandler.LogFailed("");
                }
            }
        }

        // Properly packages the DML data so that it can be transmitted to receiving server
        // Creates the proper post payload
        // changedEntity -- The inserted Record as recieved
        public static void PackagePayload(object changedEntity)
        {

        }

        public static async Task LoginAsync(string WebServerAddress, string WebServerUsername, string WebServerPassword)
        {
            
            await RecordHandler.LoginAsync(WebServerAddress, WebServerUsername, WebServerPassword);

        }

        // Takes the properly packaged payload returned from the PackagePayload method and trasmits it to receiving server
        public static void Transmit(object payload)
        {
            

        }
    }
}

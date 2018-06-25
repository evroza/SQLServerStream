using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;

namespace SQLServerNotifyStream
{
    class SQLServerStream
    {
        private static string _con = "data source=.; initial catalog=yanguTest; integrated security=True";

        // The constructor initializes the database connections and adds column mappings if any
        public SQLServerStream()
        {
            /**
             * No need to do mappings since model class properties exactly match the db column names. If otherwise then need to
                var mapper = new ModelToTableMapper<Customer>();
                mapper.AddMapping(c => c.Surname, "Second Name");
                mapper.AddMapping(c => c.Name, "First Name");
            **/

            using (var dep = new SqlTableDependency<ModelOrderProducts>(_con, tableName: "order_products"))
            {
                dep.OnChanged += Changed;
                dep.Start();

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();

                dep.Stop();
            }
        }

        public static void Changed(object sender, RecordChangedEventArgs<ModelOrderProducts> e)
        {
            var changedEntity = e.Entity;

            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine("Item ID: " + changedEntity.itemid);
            Console.WriteLine("Created at: " + changedEntity.Created_at);
            Console.WriteLine("Quantity Ordered: " + changedEntity.qty_ordered);
            Console.WriteLine("Price: " + changedEntity.price);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    class GenericDatasetJSON
    {
        // Hodls the status of the request, can either be success or error
        public string itemid { get; set; }

        public string Created_at { get; set; }

        public string qty_ordered { get; set; }

        public string price { get; set; }
    }
}

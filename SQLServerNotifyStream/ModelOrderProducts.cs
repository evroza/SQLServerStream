using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    class ModelOrderProducts
    {
        public int itemid { get; set; }
        public string Created_at { get; set; }
        public double qty_ordered { get; set; }
        public double price { get; set; }
    }
}

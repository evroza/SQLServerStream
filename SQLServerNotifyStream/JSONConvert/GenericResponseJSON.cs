using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    class GenericResponseJSON
    {
        // Hodls the status of the request, can either be success or error
        public string status { get; set; }
        // In case of error|success on login, this field is populated with descriptive message
        public string message { get; set; }
    }
}

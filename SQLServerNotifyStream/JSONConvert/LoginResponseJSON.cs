using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    class LoginResponseJSON: GenericResponseJSON
    {        
        //On login success the token field is populated
        public string token { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class Ticket {
        public string id { get; set; }
        public string status { get; set; }
        public string errorMessage { get; set; }
        public Message notification { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class GetReceiptErrorEventArgs {
        public List<Ticket> Tickets { get; set; }
        public List<GetReceiptResponseError> Errors { get; set; }
        public string ResponseBody { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}

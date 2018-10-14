using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class SendErrorEventArgs {
        public List<Message> Messages { get; set; }
        public List<SendResponseError> Errors { get; set; }
        public string ResponseBody { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}

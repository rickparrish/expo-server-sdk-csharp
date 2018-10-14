using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class SendResponse {
        public List<SendResponseData> data { get; set; }
        public List<SendResponseError> errors { get; set; }
    }

    public class SendResponseData {
        public string status { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public SendResponseDataDetails details { get; set; }
    }

    public class SendResponseDataDetails {
        public string error { get; set; }
    }

    public class SendResponseError {
        public string code { get; set; }
        public string message { get; set; }
    }
}

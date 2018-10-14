using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class GetReceiptsResponse {
        public Dictionary<string, GetReceiptsResponseData> data { get; set; }
        public List<GetReceiptResponseError> errors { get; set; }
    }

    public class GetReceiptsResponseData {
        public string status { get; set; }
        public string message { get; set; }
        public GetReceiptsResponseDataDetails details { get; set; }
    }

    public class GetReceiptsResponseDataDetails {
        public string error { get; set; }
    }

    public class GetReceiptResponseError {
        public string code { get; set; }
        public string message { get; set; }
    }
}

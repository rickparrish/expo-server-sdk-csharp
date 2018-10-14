using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// Response type used by calls to Expo's send API endpoint
    /// </summary>
    public class SendResponse {
        /// <summary>
        /// A successful request will populate the data property with tickets for each message
        /// </summary>
        public List<SendResponseData> data { get; set; }

        /// <summary>
        /// A failed request will populate the errors property
        /// </summary>
        public List<SendResponseError> errors { get; set; }
    }

    /// <summary>
    /// Ticket data for a send request
    /// </summary>
    public class SendResponseData {
        /// <summary>
        /// The status of the receipt (usually 'ok' or 'error')
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The unique id given to this ticket
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The error message (if an error occurred)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// The error details (if an error occurred)
        /// </summary>
        public SendResponseDataDetails details { get; set; }
    }

    /// <summary>
    /// Error information for a failed getReceipts call
    /// </summary>
    public class SendResponseDataDetails {
        /// <summary>
        /// Error information for a failed getReceipts call
        /// </summary>
        public string error { get; set; }
    }

    public class SendResponseError {
        /// <summary>
        /// The standardized error code for the request
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// The more descriptive error message
        /// </summary>
        public string message { get; set; }
    }
}

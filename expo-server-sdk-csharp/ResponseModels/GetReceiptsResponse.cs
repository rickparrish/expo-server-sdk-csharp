using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// Response type used by calls to Expo's getReceipts API endpoint
    /// </summary>
    public class GetReceiptsResponse {
        /// <summary>
        /// A successful request will populate the data property with receipts for each ticket,
        /// where the dictionary key is the ticket id, and the value is the receipt
        /// </summary>
        public Dictionary<string, GetReceiptsResponseData> data { get; set; }

        /// <summary>
        /// A failed request will populate the errors property
        /// </summary>
        public List<GetReceiptResponseError> errors { get; set; }
    }

    /// <summary>
    /// Receipt data for a getReceipts request
    /// </summary>
    public class GetReceiptsResponseData {
        /// <summary>
        /// The status of the receipt (usually 'ok' or 'error')
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The error message (if an error occurred)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// The error details (if an error occurred)
        /// </summary>
        public GetReceiptsResponseDataDetails details { get; set; }
    }

    /// <summary>
    /// The error details (if an error occurred)
    /// </summary>
    public class GetReceiptsResponseDataDetails {
        /// <summary>
        /// The error details (if an error occurred)
        /// </summary>
        public string error { get; set; }
    }

    /// <summary>
    /// Error information for a failed getReceipts call
    /// </summary>
    public class GetReceiptResponseError {
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

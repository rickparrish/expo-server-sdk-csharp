using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// Event arguments for a failed call to Expo's send API endpoint
    /// </summary>
    public class SendErrorEventArgs {
        /// <summary>
        /// The messages that were passed to the endpoint
        /// </summary>
        public List<Message> Messages { get; set; }

        /// <summary>
        /// The parsed errors returned by the endpoint
        /// </summary>
        public List<SendResponseError> Errors { get; set; }

        /// <summary>
        /// The raw response body returned by the endpoint
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// The Http status code returned by the endpoint
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}

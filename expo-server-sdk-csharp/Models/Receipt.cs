using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class Receipt {
        /// <summary>
        /// The id of the ticket this receipt belongs to
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The status of the recipet (usually 'ok' or 'error')
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The error message (if an error occurred)
        /// </summary>
        public string errorMessage { get; set; }

        /// <summary>
        /// The error details (if an error occurred)
        /// </summary>
        public string errorDetails { get; set; }
    }
}

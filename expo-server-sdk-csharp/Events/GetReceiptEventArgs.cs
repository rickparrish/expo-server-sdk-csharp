using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// Event arguments for a successful call to Expo's getReceipts API endpoint
    /// </summary>
    public class GetReceiptEventArgs {
        /// <summary>
        /// The receipts returned by the endpoint
        /// </summary>
        public List<Receipt> Receipts { get; set; }
    }
}

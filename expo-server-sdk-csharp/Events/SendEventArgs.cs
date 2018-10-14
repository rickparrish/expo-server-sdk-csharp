using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// Event arguments for a successful call to Expo's send API endpoint
    /// </summary>
    public class SendEventArgs {
        /// <summary>
        /// The tickets returned by the endpoint, which can be passed to Expo.GetReceipts later
        /// </summary>
        public List<Ticket> Tickets { get; set; }
    }
}

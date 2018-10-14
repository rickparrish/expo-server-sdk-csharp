using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public static class Expo {
        public static EventHandler<GetReceiptEventArgs> GetReceiptCallback = null;
        public static EventHandler<GetReceiptErrorEventArgs> GetReceiptErrorCallback = null;
        public static EventHandler<SendEventArgs> SendCallback = null;
        public static EventHandler<SendErrorEventArgs> SendErrorCallback = null;

        private const string GetReceiptsUrl = "https://exp.host/--/api/v2/push/getReceipts";
        private const string SendUrl = "https://exp.host/--/api/v2/push/send";

        private static HttpClient _Client = new HttpClient();
        private static List<Message> _MessageQueue = new List<Message>();
        private static List<Ticket> _TicketQueue = new List<Ticket>();

        static Expo() {
            _Client.DefaultRequestHeaders.Add("accept", "application/json");
            _Client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
        }

        public static async Task FlushGetReceiptAsync() {
            // Make sure we have something to flush, and bail out if not
            if (!_TicketQueue.Any()) {
                return;
            }

            try {
                // Setup our request parameters
                string JsonString = JsonConvert.SerializeObject(new {
                    ids = _TicketQueue.Where(x => !string.IsNullOrWhiteSpace(x.id)).Select(x => x.id).ToArray()
                });
                var Content = new StringContent(JsonString, Encoding.UTF8, "application/json");

                // Send the request and parse the response
                var RequestResult = _Client.PostAsync(GetReceiptsUrl, Content).Result;
                string ResponseText = await RequestResult.Content.ReadAsStringAsync();
                var ResponseObject = JsonConvert.DeserializeObject<GetReceiptsResponse>(ResponseText);

                // Generate our list of receipts
                var Receipts = new List<Receipt>();
                if (RequestResult.IsSuccessStatusCode) {
                    // Request succeeded, so we should have individual receipts
                    foreach (var KVP in ResponseObject.data) {
                        Receipts.Add(new Receipt() {
                            errorMessage = KVP.Value.message,
                            id = KVP.Key,
                            status = KVP.Value.status,
                        });
                    }

                    // Fire off the get receipt callback with the receipts
                    GetReceiptCallback?.Invoke(null, new GetReceiptEventArgs() {
                        Receipts = Receipts,
                    });
                } else {
                    // Fire off the get receipt error callback with the errors
                    GetReceiptErrorCallback?.Invoke(null, new GetReceiptErrorEventArgs() {
                        Errors = ResponseObject.errors,
                        ResponseBody = ResponseText,
                        StatusCode = RequestResult.StatusCode,
                        Tickets = _TicketQueue,
                    });
                }
            } finally {
                // Clear the queue for the next batch
                _TicketQueue.Clear();
            }
        }

        public static async Task FlushSendAsync() {
            // Make sure we have something to flush, and bail out if not
            if (!_MessageQueue.Any()) {
                return;
            }

            try {
                // Setup our request parameters
                string JsonString = JsonConvert.SerializeObject(_MessageQueue, Formatting.None, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
                var Content = new StringContent(JsonString, Encoding.UTF8, "application/json");

                // Send the request and parse the response
                var RequestResult = _Client.PostAsync(SendUrl, Content).Result;
                string ResponseText = await RequestResult.Content.ReadAsStringAsync();
                var ResponseObject = JsonConvert.DeserializeObject<SendResponse>(ResponseText);

                // Generate our list of tickets
                var Tickets = new List<Ticket>();
                if (RequestResult.IsSuccessStatusCode) {
                    // Request succeeded, so we should have individual tickets
                    for (int i = 0; i < ResponseObject.data.Count; i++) {
                        Tickets.Add(new Ticket() {
                            errorMessage = ResponseObject.data[i].message,
                            id = ResponseObject.data[i].id,
                            notification = _MessageQueue[i],
                            status = ResponseObject.data[i].status,
                        });
                    }

                    // Fire off the send callback with the tickets
                    SendCallback?.Invoke(null, new SendEventArgs() {
                        Tickets = Tickets,
                    });
                } else {
                    // Fire off the send error callback with the errors
                    SendErrorCallback?.Invoke(null, new SendErrorEventArgs() {
                        Errors = ResponseObject.errors,
                        ResponseBody = ResponseText,
                        StatusCode = RequestResult.StatusCode,
                        Messages = _MessageQueue,
                    });

                }
            } finally {
                // Clear the queue for the next batch
                _MessageQueue.Clear();
            }
        }

        public static async Task<bool> GetReceiptAsync(Ticket ticket) {
            _TicketQueue.Add(ticket);

            if (_TicketQueue.Count >= _GetReceiptBatchSize) {
                await FlushGetReceiptAsync();
                return true;
            }

            return false;
        }


        private static int _GetReceiptBatchSize = 300;
        public static int GetReceiptBatchSize {
            get {
                return _GetReceiptBatchSize;
            }
            set {
                if (value < 1) {
                    _GetReceiptBatchSize = 1;
                } else if (value > 300) {
                    _GetReceiptBatchSize = 300;
                } else {
                    _GetReceiptBatchSize = value;
                }
            }
        }

        public static async Task<bool> SendAsync(Message message) {
            _MessageQueue.Add(message);

            if (_MessageQueue.Count >= _SendBatchSize) {
                await FlushSendAsync();
                return true;
            }

            return false;
        }

        private static int _SendBatchSize = 100;
        public static int SendBatchSize {
            get {
                return _SendBatchSize;
            }
            set {
                if (value < 1) {
                    _SendBatchSize = 1;
                } else if (value > 100) {
                    _SendBatchSize = 100;
                } else {
                    _SendBatchSize = value;
                }
            }
        }
    }
}

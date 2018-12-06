using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    /// <summary>
    /// A C# implementation of Expo's Push API
    /// </summary>
    public class Expo {
        /// <summary>
        /// This callback is invoked when Expo's getReceipts API is called successfully, and receipts
        /// for previously delivered messages are provided
        /// </summary>
        public EventHandler<GetReceiptEventArgs> GetReceiptCallback = null;

        /// <summary>
        /// This callback is invoked when Expo's getReceipts API returns a non-successful status code
        /// </summary>
        public EventHandler<GetReceiptErrorEventArgs> GetReceiptErrorCallback = null;

        /// <summary>
        /// This callback is invoked when Expo's push API is called successfully, meaning messages were
        /// queued up for delivery and tickets to check receipt status later are provided
        /// </summary>
        public EventHandler<SendEventArgs> SendCallback = null;

        /// <summary>
        /// This callback is invoked when Expo's send API returns a non-successful status code
        /// </summary>
        public EventHandler<SendErrorEventArgs> SendErrorCallback = null;

        private const string GetReceiptsUrl = "https://exp.host/--/api/v2/push/getReceipts";
        private const string SendUrl = "https://exp.host/--/api/v2/push/send";

        private HttpClient _Client = new HttpClient();
        private List<Message> _MessageQueue = new List<Message>();
        private List<Ticket> _TicketQueue = new List<Ticket>();

        public Expo() {
            _Client.DefaultRequestHeaders.Add("accept", "application/json");
            _Client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
        }

        /// <summary>
        /// Initiates a request to Expo's getReceipts API endpoint, passing any tickets currently queued up
        /// by previous calls to Expo.GetReceipt.
        /// 
        /// Expo.GetReceipt will only queue up the tickets, but will not actually make a call to Expo's getReceipts
        /// API endpoint until the queue limit has been reached (defaults to 300).  So if you call Expo.GetReceipt
        /// 50 times, there will be 50 items waiting in the queue that you need to call FlushGetReceipt to process.
        /// 
        /// Or if you call Expo.GetReceipt 325 times, then a batch of 300 will have been handled and 25 will items
        /// will be waiting in the queue that you need to call FlushGetReceipt to process.
        /// </summary>
        public async Task FlushGetReceiptAsync() {
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

                // Catch deserialize errors so we can report invalid responsetext
                GetReceiptsResponse ResponseObject = null;
                try {
                    ResponseObject = JsonConvert.DeserializeObject<GetReceiptsResponse>(ResponseText);
                } catch (Exception ex) {
                    // Fire off the get receipt error callback with the errors
                    GetReceiptErrorCallback?.Invoke(null, new GetReceiptErrorEventArgs() {
                        Errors = new List<GetReceiptResponseError>() {
                            new GetReceiptResponseError() {
                                code = "-1",
                                message = ex.Message
                            }
                        },
                        ResponseBody = ResponseText,
                        StatusCode = RequestResult.StatusCode,
                        Tickets = _TicketQueue,
                    });

                    return;
                }

                if (RequestResult.IsSuccessStatusCode) {
                    // Request succeeded, so we should have individual receipts
                    var Receipts = new List<Receipt>();
                    foreach (var KVP in ResponseObject.data) {
                        Receipts.Add(new Receipt() {
                            errorDetails = KVP.Value.details?.error,
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

        /// <summary>
        /// Initiates a request to Expo's send API endpoint, passing any messages currently queued up
        /// by previous calls to Expo.Send
        /// 
        /// Expo.Send will only queue up the messages, but will not actually make a call to Expo's send
        /// API endpoint until the queue limit has been reached (defaults to 100).  So if you call Expo.Send
        /// 50 times, there will be 50 items waiting in the queue that you need to call FlushSend to process.
        /// 
        /// Or if you call Expo.Send 125 times, then a batch of 100 will have been handled and 25 will items
        /// will be waiting in the queue that you need to call FlushSend to process.
        /// </summary>
        public async Task FlushSendAsync() {
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

                // Catch deserialize errors so we can report invalid responsetext
                SendResponse ResponseObject = null;
                try {
                    ResponseObject = JsonConvert.DeserializeObject<SendResponse>(ResponseText);
                } catch (Exception ex) {
                    // Fire off the send error callback with the errors
                    SendErrorCallback?.Invoke(null, new SendErrorEventArgs() {
                        Errors = new List<SendResponseError>() {
                            new SendResponseError() {
                                code = "-1",
                                message = ex.Message
                            }
                        },
                        ResponseBody = ResponseText,
                        StatusCode = RequestResult.StatusCode,
                        Messages = _MessageQueue,
                    });

                    return;
                }

                // Generate our list of tickets
                if (RequestResult.IsSuccessStatusCode) {
                    // Request succeeded, so we should have individual tickets
                    var Tickets = new List<Ticket>();
                    for (int i = 0; i < ResponseObject.data.Count; i++) {
                        Tickets.Add(new Ticket() {
                            errorDetails = ResponseObject.data[i].details?.error,
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

        /// <summary>
        /// Queues up an individual ticket for later retrieval via Expo's getReceipts API endpoint
        /// </summary>
        /// <param name="ticket">The ticket to request receipt information for</param>
        /// <returns>true if the ticket was processed (batch size was reached), false if it was queued</returns>
        public async Task<bool> GetReceiptAsync(Ticket ticket) {
            _TicketQueue.Add(ticket);

            if (_TicketQueue.Count >= _GetReceiptBatchSize) {
                await FlushGetReceiptAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves a batch of tickets via Expo's getReceipts API endpoint
        /// 
        /// Unlike the individual-ticket-handling Expo.GetReceipt method, this one is guaranteed to process
        /// all the tickets provided by automatically calling Expo.FlushGetReceipt at the end, so you don't
        /// need to manually call that yourself
        /// </summary>
        /// <param name="tickets">The tickets to request receipt information for</param>
        public async Task GetReceiptsAsync(List<Ticket> tickets) {
            foreach (var Ticket in tickets) {
                await GetReceiptAsync(Ticket);
            }
            await FlushGetReceiptAsync();
        }

        /// <summary>
        /// Determines how many tickets are passed to Expo's getReceipts API endpoint at once.
        /// The default (and maximum) value is 300, and it's recommended to leave this alone for
        /// minimum HTTP querying overhead
        /// </summary>
        public int GetReceiptBatchSize {
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
        private int _GetReceiptBatchSize = 300;

        /// <summary>
        /// Queues up an individual message for later delivery via Expo's send API endpoint
        /// </summary>
        /// <param name="message">The message to deliver</param>
        /// <returns>true if the message was processed (batch size was reached), false if it was queued</returns>
        public async Task<bool> SendAsync(Message message) {
            _MessageQueue.Add(message);

            if (_MessageQueue.Count >= _SendBatchSize) {
                await FlushSendAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines how many messages are passed to Expo's send API endpoint at once.
        /// The default (and maximum) value is 100, and it's recommended to leave this alone for
        /// minimum HTTP querying overhead
        /// </summary>
        public int SendBatchSize {
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
        private int _SendBatchSize = 100;
    }
}

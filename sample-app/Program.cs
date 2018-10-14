using expo_server_sdk_csharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_app {
    class Program {
        const string PushTokenFilename = @".\push-tokens.txt";

        private static List<Ticket> _Tickets = new List<Ticket>();

        static void Main(string[] args) {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args) { 
            // Ensure we have the input file
            if (!File.Exists(PushTokenFilename)) {
                Console.WriteLine("Please create a push-tokens.txt containing the tokens you'd like to test with.");
                Console.WriteLine("Place one token on each line of the file.");
                Console.WriteLine();
                Console.WriteLine("Hit a key to quit");
                Console.ReadKey();
                return;
            }

            // Setup Expo
            Expo.SendBatchSize = 2; // Could be anywhere from 1 to 100, using 2 for testing purposes
            Expo.SendCallback += Expo_SendCallback;
            Expo.SendErrorCallback += Expo_SendErrorCallback;
            Expo.GetReceiptBatchSize = 2; // Could be anywhere from 1 to 300, using 2 for testing purposes
            Expo.GetReceiptCallback += Expo_GetReceiptCallback;
            Expo.GetReceiptErrorCallback += Expo_GetReceiptErrorCallback;

            // Read the tokens from the input file and loop through them
            try {
                string[] PushTokens = File.ReadAllLines(PushTokenFilename);
                foreach (string PushToken in PushTokens) {
                    Console.WriteLine($"Handling {PushToken}...");
                    await Expo.SendAsync(new Message() {
                        body = DateTime.Now.ToString(),
                        title = "Hello from EvidenceAlerts",
                        to = PushToken,
                    });
                }
            } finally {
                // Always call FlushSendAsync at the end to ensure any queued messages are delivered
                await Expo.FlushSendAsync(); 
            }

            // Pause until user is ready to check receipts
            Console.WriteLine();
            Console.WriteLine("Send completed, wait a bit and hit a key to check the receipts");
            Console.ReadKey();

            try {
                foreach (var Ticket in _Tickets) {
                    if (string.IsNullOrWhiteSpace((string)Ticket.id)) {
                        Console.WriteLine($"Message to {Ticket.notification.to} failed...");
                        Console.WriteLine($"  Status: {Ticket.status}");
                        Console.WriteLine($"  Message: {Ticket.errorMessage}");
                    } else {
                        Console.WriteLine($"Checking {Ticket.id}...");
                        await Expo.GetReceiptAsync(Ticket);
                    }
                }
            } finally {
                // Always call FlushGetReceiptAsync at the end to ensure any queued requests are delivered
                await Expo.FlushGetReceiptAsync();
            }

            // Pause at end
            Console.WriteLine();
            Console.WriteLine("Hit a key to quit");
            Console.ReadKey();
        }

        private static void Expo_GetReceiptCallback(object sender, GetReceiptEventArgs e) {
            // Output to screen so we can see what we're receiving
            Console.WriteLine();
            Console.WriteLine("GetReceipts Callback...");
            foreach (var Receipt in e.Receipts) {
                Console.WriteLine(JsonConvert.SerializeObject(Receipt));
            }
            Console.WriteLine();
        }

        private static void Expo_GetReceiptErrorCallback(object sender, GetReceiptErrorEventArgs e) {
            // Output errors to screen
            Console.WriteLine();
            Console.WriteLine("Call to GetReceipts failed...");
            Console.WriteLine($"  HttpStatus: {e.StatusCode}");
            Console.WriteLine($"  Response: {e.ResponseBody}");
            Console.WriteLine();
        }

        private static void Expo_SendCallback(object sender, SendEventArgs e) {
            // Store the tickets for querying later
            _Tickets.AddRange(e.Tickets);

            // Output to screen so we can see what we're receiving
            Console.WriteLine();
            Console.WriteLine("Send Callback...");
            foreach (var Ticket in e.Tickets) {
                Console.WriteLine(JsonConvert.SerializeObject(Ticket));
            }
            Console.WriteLine();
        }

        private static void Expo_SendErrorCallback(object sender, SendErrorEventArgs e) {
            // Output errors to screen
            Console.WriteLine();
            Console.WriteLine("Call to Send failed...");
            Console.WriteLine($"  HttpStatus: {e.StatusCode}");
            Console.WriteLine($"  Response: {e.ResponseBody}");
            Console.WriteLine();
        }
    }
}

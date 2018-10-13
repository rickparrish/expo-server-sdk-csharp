using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_app {
    class Program {
        const string PushTokenFilename = @".\push-tokens.txt";
        static void Main(string[] args) {
            // Ensure we have the input file
            if (!File.Exists(PushTokenFilename)) {
                Console.WriteLine("Please create a push-tokens.txt containing the tokens you'd like to test with.");
                Console.WriteLine("Place one token on each line of the file.");
                Console.WriteLine();
                Console.WriteLine("Hit a key to quit");
                Console.ReadKey();
                return;
            }

            // Read the tokens from the input file and loop through them
            string[] PushTokens = File.ReadAllLines(PushTokenFilename);
            foreach (string PushToken in PushTokens) {
                Console.WriteLine($"Handling {PushToken}...");
            }

            // Pause at end
            Console.WriteLine();
            Console.WriteLine("Hit a key to quit");
            Console.ReadKey();
        }
    }
}

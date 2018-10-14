using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class Message {
        public string to { get; set; }
        public string data { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int? ttl { get; set; }
        public int? expiration { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Priority? priority { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Sound? sound { get; set; }
        public int? badge { get; set; }
        public string channelId { get; set; }
    }
}

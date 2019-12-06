using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Chat_Client.Models
{
    internal class MemberMessageModel
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    internal record WhiteBitToken
    {
        [JsonPropertyName("websocket_token")]
        public string Token { get; set; } = string.Empty;
    }
}

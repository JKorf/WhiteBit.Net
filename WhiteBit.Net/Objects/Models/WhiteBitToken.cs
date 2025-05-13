using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    [SerializationModel]
    internal record WhiteBitToken
    {
        [JsonPropertyName("websocket_token")]
        public string Token { get; set; } = string.Empty;
    }
}

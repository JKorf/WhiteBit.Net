using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    internal record WhiteBitSubscribeResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}

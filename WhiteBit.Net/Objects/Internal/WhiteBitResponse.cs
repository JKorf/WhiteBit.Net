using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    internal class WhiteBitResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("result")]
        public T? Result { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}

using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    [SerializationModel]
    internal record WhiteBitSocketResponse<T>
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("error")]
        public WhiteBitSocketError? Error { get; set; }
        [JsonPropertyName("result")]
        public T Result { get; set; } = default!;
    }

    [SerializationModel]
    internal record WhiteBitSocketError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
}

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

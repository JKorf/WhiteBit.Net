using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    internal record WhiteBitOffsetResult<T>
    {
        [JsonPropertyName("records")]
        public IEnumerable<T> Records { get; set; } = [];
    }
}

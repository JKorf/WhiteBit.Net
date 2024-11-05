using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Order update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record WhiteBitOrderUpdate
    {
        /// <summary>
        /// Event
        /// </summary>
        [ArrayProperty(0)]
        [JsonConverter(typeof(EnumConverter))]
        public OrderEvent Event { get; set; }
        /// <summary>
        /// Order info
        /// </summary>
        [ArrayProperty(1)]
        public WhiteBitOrder Order { get; set; } = null!;
    }
}

using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WhiteBit.Net.Converters;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// OTO order update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitOtoOrderUpdate, WhiteBitSourceGenerationContext>))]
    [SerializationModel]
    public record WhiteBitOtoOrderUpdate
    {
        /// <summary>
        /// Event
        /// </summary>
        [ArrayProperty(0)]

        public OrderEvent Event { get; set; }
        /// <summary>
        /// Order info
        /// </summary>
        [ArrayProperty(1)]
        public WhiteBitOtoOrder Order { get; set; } = null!;
    }
}

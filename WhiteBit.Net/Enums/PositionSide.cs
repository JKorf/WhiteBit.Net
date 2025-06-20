using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Position side
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PositionSide>))]
    public enum PositionSide
    {
        /// <summary>
        /// Long position
        /// </summary>
        [Map("LONG")]
        Long,
        /// <summary>
        /// Short position
        /// </summary>
        [Map("SHORT")]
        Short,
        /// <summary>
        /// Both
        /// </summary>
        [Map("BOTH")]
        Both
    }
}

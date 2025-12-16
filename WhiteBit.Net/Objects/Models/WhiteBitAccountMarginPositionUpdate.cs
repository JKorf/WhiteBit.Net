using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Account borrow update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitAccountMarginPositionUpdate>))]
    [SerializationModel]
    public record WhiteBitAccountMarginPositionUpdate
    {
        /// <summary>
        /// Update type
        /// </summary>
        [ArrayProperty(0)]
        public AccountBorrowUpdateType UpdateType { get; set; }
        /// <summary>
        /// The position info the update relates to
        /// </summary>
        [ArrayProperty(1)]
        public WhiteBitPosition PositionInfo { get; set; } = default!;
    }


}

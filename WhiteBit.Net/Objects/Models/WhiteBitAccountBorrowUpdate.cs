using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Account borrow update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitAccountBorrowUpdate>))]
    [SerializationModel]
    public record WhiteBitAccountBorrowUpdate
    {
        /// <summary>
        /// Update type
        /// </summary>
        [ArrayProperty(0)]
        public AccountBorrowUpdateType UpdateType { get; set; }
        /// <summary>
        /// The borrow info the update relates to
        /// </summary>
        [ArrayProperty(1)]
        public WhiteBitBorrow BorrowInfo { get; set; } = default!;
    }


}

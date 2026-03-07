using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Symbol info
    /// </summary>
    [SerializationModel]
    public record WhiteBitSymbol
    {
        /// <summary>
        /// ["<c>name</c>"] Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>stock</c>"] Base asset
        /// </summary>
        [JsonPropertyName("stock")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>money</c>"] Quote asset
        /// </summary>
        [JsonPropertyName("money")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>stockPrec</c>"] Base asset precision
        /// </summary>
        [JsonPropertyName("stockPrec")]
        public int BaseAssetPrecision { get; set; }
        /// <summary>
        /// ["<c>moneyPrec</c>"] Quote asset precision
        /// </summary>
        [JsonPropertyName("moneyPrec")]
        public int QuoteAssetPrecision { get; set; }
        /// <summary>
        /// ["<c>feePrec</c>"] Fee precision
        /// </summary>
        [JsonPropertyName("feePrec")]
        public decimal FeePrecision { get; set; }
        /// <summary>
        /// ["<c>makerFee</c>"] Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// ["<c>takerFee</c>"] Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// ["<c>minAmount</c>"] Min order quantity
        /// </summary>
        [JsonPropertyName("minAmount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// ["<c>minTotal</c>"] Min order value
        /// </summary>
        [JsonPropertyName("minTotal")]
        public decimal MinOrderValue { get; set; }
        /// <summary>
        /// ["<c>maxTotal</c>"] Max order value
        /// </summary>
        [JsonPropertyName("maxTotal")]
        public decimal MaxOrderValue { get; set; }
        /// <summary>
        /// ["<c>tradesEnabled</c>"] If trading is enabled
        /// </summary>
        [JsonPropertyName("tradesEnabled")]
        public bool TradingEnabled { get; set; }
        /// <summary>
        /// ["<c>isCollateral</c>"] Is margin trading enabled
        /// </summary>
        [JsonPropertyName("isCollateral")]
        public bool IsMarginEnabled { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Type of symbol
        /// </summary>
        [JsonPropertyName("type")]
        public SymbolType SymbolType { get; set; }
    }


}

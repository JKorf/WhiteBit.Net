using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Symbol info
    /// </summary>
    public record WhiteBitSymbol
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Base asset
        /// </summary>
        [JsonPropertyName("stock")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// Quote asset
        /// </summary>
        [JsonPropertyName("money")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// Base asset precision
        /// </summary>
        [JsonPropertyName("stockPrec")]
        public int BaseAssetPrecision { get; set; }
        /// <summary>
        /// Quote asset precision
        /// </summary>
        [JsonPropertyName("moneyPrec")]
        public int QuoteAssetPrecision { get; set; }
        /// <summary>
        /// Fee precision
        /// </summary>
        [JsonPropertyName("feePrec")]
        public decimal FeePrecision { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// Min order quantity
        /// </summary>
        [JsonPropertyName("minAmount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// Min order value
        /// </summary>
        [JsonPropertyName("minTotal")]
        public decimal MinOrderValue { get; set; }
        /// <summary>
        /// Max order value
        /// </summary>
        [JsonPropertyName("maxTotal")]
        public decimal MaxOrderValue { get; set; }
        /// <summary>
        /// If trading is enabled
        /// </summary>
        [JsonPropertyName("tradesEnabled")]
        public bool TradingEnabled { get; set; }
        /// <summary>
        /// Is margin trading enabled
        /// </summary>
        [JsonPropertyName("isCollateral")]
        public bool IsMarginEnabled { get; set; }
        /// <summary>
        /// Type of symbol
        /// </summary>
        [JsonPropertyName("type")]
        public SymbolType SymbolType { get; set; }
    }


}

using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Positions
    /// </summary>
    [SerializationModel]
    public record WhiteBitPositionsUpdate
    {
        /// <summary>
        /// ["<c>total</c>"] Total
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
        /// <summary>
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitPositionUpdate[] Records { get; set; } = Array.Empty<WhiteBitPositionUpdate>();
    }

    /// <summary>
    /// Position info
    /// </summary>
    [SerializationModel]
    public record WhiteBitPositionUpdate
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ctime</c>"] Create time
        /// </summary>
        [JsonPropertyName("ctime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>mtime</c>"] Update time
        /// </summary>
        [JsonPropertyName("mtime")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Position quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>amount_in_money</c>"] Position value
        /// </summary>
        [JsonPropertyName("amount_in_money")]
        public decimal Value { get; set; }
        /// <summary>
        /// ["<c>base_price</c>"] Base price
        /// </summary>
        [JsonPropertyName("base_price")]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// ["<c>pnl</c>"] Profit and loss
        /// </summary>
        [JsonPropertyName("pnl")]
        public decimal UnrealizedPnl { get; set; }
        /// <summary>
        /// ["<c>liq_price</c>"] Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
        /// <summary>
        /// ["<c>liq_stage</c>"] Liquidation stage
        /// </summary>
        [JsonPropertyName("liq_stage")]
        public string? LiquidationStage { get; set; }
        /// <summary>
        /// ["<c>unrealized_funding</c>"] Unrealized funding
        /// </summary>
        [JsonPropertyName("unrealized_funding")]
        public decimal UnrealizedFunding { get; set; }
        /// <summary>
        /// ["<c>funding</c>"] Funding
        /// </summary>
        [JsonPropertyName("funding")]
        public decimal Funding { get; set; }
        /// <summary>
        /// ["<c>margin</c>"] Margin
        /// </summary>
        [JsonPropertyName("margin")]
        public decimal Margin { get; set; }
        /// <summary>
        /// ["<c>free_margin</c>"] Free margin
        /// </summary>
        [JsonPropertyName("free_margin")]
        public decimal FreeMargin { get; set; }
        /// <summary>
        /// ["<c>realized_pnl</c>"] Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal RealizedPnl { get; set; }
        /// <summary>
        /// ["<c>position_side</c>"] Position side
        /// </summary>
        [JsonPropertyName("position_side")]
        public PositionSide PositionSide { get; set; }
    }


}

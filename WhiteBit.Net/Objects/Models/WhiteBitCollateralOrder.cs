using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Collateral order info
    /// </summary>
    [SerializationModel]
    public record WhiteBitCollateralOrder
    {
        /// <summary>
        /// ["<c>orderId</c>"] Order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        /// <summary>
        /// ["<c>clientOrderId</c>"] Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>side</c>"] Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide OrderSide { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderType OrderType { get; set; }
        /// <summary>
        /// ["<c>timestamp</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// ["<c>dealMoney</c>"] Filled quantity in quote asset
        /// </summary>
        [JsonPropertyName("dealMoney")]
        public decimal QuoteQuantityFilled { get; set; }
        /// <summary>
        /// ["<c>dealStock</c>"] Filled quantity
        /// </summary>
        [JsonPropertyName("dealStock")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>takerFee</c>"] Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// ["<c>makerFee</c>"] Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// ["<c>left</c>"] Left
        /// </summary>
        [JsonPropertyName("left")]
        public decimal Left { get; set; }
        /// <summary>
        /// ["<c>dealFee</c>"] Deal fee
        /// </summary>
        [JsonPropertyName("dealFee")]
        public decimal DealFee { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// ["<c>postOnly</c>"] Post only
        /// </summary>
        [JsonPropertyName("postOnly")]
        public bool PostOnly { get; set; }
        /// <summary>
        /// ["<c>ioc</c>"] Immediate or cancel
        /// </summary>
        [JsonPropertyName("ioc")]
        public bool ImmediateOrCancel { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Closed order status
        /// </summary>
        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }
        /// <summary>
        /// ["<c>stp</c>"] Self Trade Prevention mode
        /// </summary>
        [JsonPropertyName("stp")]
        public SelfTradePreventionMode StpMode { get; set; }
        /// <summary>
        /// ["<c>positionSide</c>"] Position side
        /// </summary>
        [JsonPropertyName("positionSide")]
        public PositionSide PositionSide { get; set; }
        /// <summary>
        /// ["<c>oto</c>"] Take profit or stop loss configuration
        /// </summary>
        [JsonPropertyName("oto")]
        public WhiteBitCollateralOrderConfig? OneTriggersOtherConfig { get; set; }
    }

    /// <summary>
    /// OTO config
    /// </summary>
    [SerializationModel]
    public record WhiteBitCollateralOrderConfig
    {
        /// <summary>
        /// ["<c>otoId</c>"] OTO id
        /// </summary>
        [JsonPropertyName("otoId")]
        public long OtoId { get; set; }
        /// <summary>
        /// ["<c>stopLoss</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// ["<c>takeProfit</c>"] Take profit price
        /// </summary>
        [JsonPropertyName("takeProfit")]
        public decimal? TakeProfitPrice { get; set; }
    }


}

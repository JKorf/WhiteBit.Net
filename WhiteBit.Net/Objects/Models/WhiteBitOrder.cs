using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Orders
    /// </summary>
    [SerializationModel]
    public record WhiteBitOrders
    {
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitOrder[] Records { get; set; } = Array.Empty<WhiteBitOrder>();
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Order info
    /// </summary>
    [SerializationModel]
    public record WhiteBitOrder
    {
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonInclude, JsonPropertyName("id")]
        internal long OrderIdInt { set => OrderId = value; }
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        [JsonInclude, JsonPropertyName("client_order_id")]
        internal string? ClientOrderIdInt { set => ClientOrderId = value; }
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide OrderSide { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Create timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime CreateTime { get; set; }

        [JsonInclude, JsonPropertyName("ctime")]
        internal DateTime CreateTimeInt { set { CreateTime = value; } }

        /// <summary>
        /// Fill timestamp
        /// </summary>
        [JsonPropertyName("ftime")]
        public DateTime? FillTime { get; set; }
        /// <summary>
        /// Edit timestamp
        /// </summary>
        [JsonPropertyName("mtime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Filled quantity in quote asset
        /// </summary>
        [JsonPropertyName("dealMoney")]
        public decimal QuoteQuantityFilled { get; set; }
        [JsonInclude, JsonPropertyName("deal_money")]
        internal decimal QuoteQuantityFilledInt { set => QuoteQuantityFilled = value; }
        /// <summary>
        /// Filled quantity
        /// </summary>
        [JsonPropertyName("dealStock")]
        public decimal QuantityFilled { get; set; }
        [JsonInclude, JsonPropertyName("deal_stock")]
        internal decimal QuantityFilledInt { set => QuantityFilled = value; }
        /// <summary>
        /// Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal? TakerFee { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal? MakerFee { get; set; }
        /// <summary>
        /// Remaining quantity to be filled
        /// </summary>
        [JsonPropertyName("left")]
        public decimal? QuantityRemaining { get; set; }
        /// <summary>
        /// Order fee
        /// </summary>
        [JsonPropertyName("dealFee")]
        public decimal Fee { get; set; }
        [JsonInclude, JsonPropertyName("deal_fee")]
        internal decimal FeeInt { set => Fee = value; }
        /// <summary>
        /// Fee asset
        /// </summary>
        [JsonPropertyName("fee_asset")]
        public string? FeeAsset { get; set; }
        /// <summary>
        /// Limit price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// Post only
        /// </summary>
        [JsonPropertyName("postOnly")]
        public bool PostOnly { get; set; }
        [JsonInclude, JsonPropertyName("post_only")]
        internal bool PostOnlyInt { set => PostOnly = value; }
        /// <summary>
        /// Immediate or cancel flag
        /// </summary>
        [JsonPropertyName("ioc")]
        public bool? ImmediateOrCancel { get; set; }
        /// <summary>
        /// Trigger price
        /// </summary>
        [JsonPropertyName("activation_price")]
        public decimal? TriggerPrice { get; set; }

        /// <summary>
        /// Closed order status
        /// </summary>
        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }
        /// <summary>
        /// Self Trade Prevention mode
        /// </summary>
        [JsonPropertyName("stp")]
        public SelfTradePreventionMode StpMode { get; set; }
        /// <summary>
        /// Position side
        /// </summary>
        [JsonPropertyName("position_side")]
        public PositionSide? PositionSide { get; set; }

        /// <summary>
        /// Order source
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// Take profit / stop loss data, not used for websocket updates
        /// </summary>
        [JsonPropertyName("oto")]
        public WhiteBitOto? OtoData { get; set; }
    }

    /// <summary>
    /// OTO info
    /// </summary>
    public record WhiteBitOto
    {
        /// <summary>
        /// OTO Id
        /// </summary>
        [JsonPropertyName("otoId")]
        public long OtoId { get; set; }
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal? StopLoss { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("takeProfit")]
        public decimal? TakeProfit { get; set; }
    }

    /// <summary>
    /// Conditional order
    /// </summary>
    [SerializationModel]
    public record WhiteBitConditionalOrder : WhiteBitOrder
    {
        /// <summary>
        /// Trigger condition
        /// </summary>
        [JsonPropertyName("activation_condition")]
        public ActivationCondition? TriggerCondition { get; set; }
        /// <summary>
        /// Activated
        /// </summary>
        [JsonPropertyName("activated")]
        public bool? Activated { get; set; }
    }

    /// <summary>
    /// Orders
    /// </summary>
    [SerializationModel]
    public record WhiteBitClosedOrders
    {
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitClosedOrder[] Records { get; set; } = Array.Empty<WhiteBitClosedOrder>();
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Closed order info
    /// </summary>
    [SerializationModel]
    public record WhiteBitClosedOrder : WhiteBitOrder
    {
    }
}

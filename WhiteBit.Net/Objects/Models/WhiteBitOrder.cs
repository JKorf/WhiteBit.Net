using CryptoExchange.Net.Converters.SystemTextJson;
using System;
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
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitOrder[] Records { get; set; } = Array.Empty<WhiteBitOrder>();
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// ["<c>total</c>"] Total
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
        /// ["<c>orderId</c>"] Order id
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonInclude, JsonPropertyName("id")]
        internal long OrderIdInt { set => OrderId = value; }
        /// <summary>
        /// ["<c>clientOrderId</c>"] Client order id
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string? ClientOrderId { get; set; }
        [JsonInclude, JsonPropertyName("client_order_id")]
        internal string? ClientOrderIdInt { set => ClientOrderId = value; }
        /// <summary>
        /// ["<c>market</c>"] Symbol name
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
        /// ["<c>timestamp</c>"] Create timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime CreateTime { get; set; }

        [JsonInclude, JsonPropertyName("ctime")]
        internal DateTime CreateTimeInt { set { CreateTime = value; } }

        /// <summary>
        /// ["<c>ftime</c>"] Fill timestamp
        /// </summary>
        [JsonPropertyName("ftime")]
        public DateTime? FillTime { get; set; }
        /// <summary>
        /// ["<c>mtime</c>"] Edit timestamp
        /// </summary>
        [JsonPropertyName("mtime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// ["<c>dealMoney</c>"] Filled quantity in quote asset
        /// </summary>
        [JsonPropertyName("dealMoney")]
        public decimal QuoteQuantityFilled { get; set; }
        [JsonInclude, JsonPropertyName("deal_money")]
        internal decimal QuoteQuantityFilledInt { set => QuoteQuantityFilled = value; }
        /// <summary>
        /// ["<c>dealStock</c>"] Filled quantity
        /// </summary>
        [JsonPropertyName("dealStock")]
        public decimal QuantityFilled { get; set; }
        [JsonInclude, JsonPropertyName("deal_stock")]
        internal decimal QuantityFilledInt { set => QuantityFilled = value; }
        /// <summary>
        /// ["<c>amount</c>"] Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>takerFee</c>"] Taker fee
        /// </summary>
        [JsonPropertyName("takerFee")]
        public decimal? TakerFee { get; set; }
        /// <summary>
        /// ["<c>makerFee</c>"] Maker fee
        /// </summary>
        [JsonPropertyName("makerFee")]
        public decimal? MakerFee { get; set; }
        /// <summary>
        /// ["<c>left</c>"] Remaining quantity to be filled
        /// </summary>
        [JsonPropertyName("left")]
        public decimal? QuantityRemaining { get; set; }
        /// <summary>
        /// ["<c>dealFee</c>"] Order fee
        /// </summary>
        [JsonPropertyName("dealFee")]
        public decimal Fee { get; set; }
        [JsonInclude, JsonPropertyName("deal_fee")]
        internal decimal FeeInt { set => Fee = value; }
        /// <summary>
        /// ["<c>fee_asset</c>"] Fee asset
        /// </summary>
        [JsonPropertyName("fee_asset")]
        public string? FeeAsset { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Limit price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// ["<c>postOnly</c>"] Post only
        /// </summary>
        [JsonPropertyName("postOnly")]
        public bool PostOnly { get; set; }
        [JsonInclude, JsonPropertyName("post_only")]
        internal bool PostOnlyInt { set => PostOnly = value; }
        /// <summary>
        /// ["<c>ioc</c>"] Immediate or cancel flag
        /// </summary>
        [JsonPropertyName("ioc")]
        public bool? ImmediateOrCancel { get; set; }
        /// <summary>
        /// ["<c>activation_price</c>"] Trigger price
        /// </summary>
        [JsonPropertyName("activation_price")]
        public decimal? TriggerPrice { get; set; }

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
        /// ["<c>position_side</c>"] Position side
        /// </summary>
        [JsonPropertyName("position_side")]
        public PositionSide? PositionSide { get; set; }

        /// <summary>
        /// ["<c>source</c>"] Order source
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// ["<c>oto</c>"] Take profit / stop loss data, not used for websocket updates
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
        /// ["<c>otoId</c>"] OTO Id
        /// </summary>
        [JsonPropertyName("otoId")]
        public long OtoId { get; set; }
        /// <summary>
        /// ["<c>stopLoss</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stopLoss")]
        public decimal? StopLoss { get; set; }
        /// <summary>
        /// ["<c>takeProfit</c>"] Take profit price
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
        /// ["<c>activation_condition</c>"] Trigger condition
        /// </summary>
        [JsonPropertyName("activation_condition")]
        public ActivationCondition? TriggerCondition { get; set; }
        /// <summary>
        /// ["<c>activated</c>"] Activated
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
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitClosedOrder[] Records { get; set; } = Array.Empty<WhiteBitClosedOrder>();
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// ["<c>total</c>"] Total
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

using CryptoExchange.Net.Objects;
using System;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects.Sockets;
using WhiteBit.Net.Objects.Models;
using System.Collections.Generic;
using WhiteBit.Net.Enums;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 streams
    /// </summary>
    public interface IWhiteBitSocketClientV4Api : ISocketApiClient<WhiteBitCredentials>, IDisposable
    {
        /// <summary>
        /// Place a new spot order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-limit-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-market-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-market-stock-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-stop-limit-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-stop-market-place" /><br />
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETH_USDT`</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity of the order in base asset</param>
        /// <param name="quoteQuantity">["<c>amount</c>"] Quantity of the order in quote asset, only supported for market buy orders</param>
        /// <param name="price">["<c>price</c>"] Limit price</param>
        /// <param name="postOnly">["<c>post_only</c>"] Post only flag</param>
        /// <param name="immediateOrCancel">["<c>ioc</c>"] Immediate or cancel flag</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] Trigger price</param>
        /// <param name="clientOrderId">["<c>client_order_id</c>"] Client order id</param>
        /// <param name="stpMode">["<c>stp</c>"] Self trade prevention mode</param>
        /// <param name="rpi">["<c>rpi</c>"] Enables Retail Price Improvement (RPI) mode.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitOrder>> PlaceSpotOrderAsync(
            string symbol,
            OrderSide side,
            NewOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            bool? postOnly = null,
            bool? immediateOrCancel = null,
            decimal? triggerPrice = null,
            string? clientOrderId = null,
            SelfTradePreventionMode? stpMode = null,
            bool? rpi = null,
            CancellationToken ct = default
            );

        /// <summary>
        /// Place a new collateral order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/collateral-order-limit-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/collateral-order-market-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/collateral-order-stop-limit-place" /><br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/collateral-order-stop-market-place" /><br />
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity of the order in base asset</param>
        /// <param name="price">["<c>price</c>"] Limit price</param>
        /// <param name="postOnly">["<c>postOnly</c>"] Post only flag</param>
        /// <param name="immediateOrCancel">["<c>ioc</c>"] Immediate or cancel flag</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] Trigger price</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Client order id</param>
        /// <param name="stpMode">["<c>stp</c>"] Self trade prevention mode</param>
        /// <param name="positionSide">["<c>positionSide</c>"] Position side, required when in hedge mode</param>
        /// <param name="reduceOnly">["<c>reduceOnly</c>"] Reduce only order</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitCollateralOrder>> PlaceCollateralOrderAsync(
            string symbol,
            OrderSide side,
            NewOrderType type,
            decimal? quantity = null,
            decimal? price = null,
            bool? postOnly = null,
            bool? immediateOrCancel = null,
            decimal? triggerPrice = null,
            string? clientOrderId = null,
            SelfTradePreventionMode? stpMode = null,
            PositionSide? positionSide = null,
            bool? reduceOnly = null,
            CancellationToken ct = default);

        /// <summary>
        /// Set take profit and stop loss for a collateral position
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/collateral-order-tpsl-place" /><br />
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="takeProfitPrice">["<c>take_profit_activation_price</c>"]</param>
        /// <param name="stopLossPrice">["<c>stop_loss_activation_price</c>"]</param>
        /// <param name="positionSide">["<c>position_side</c>"]</param>
        /// <param name="stpMode">["<c>stp</c>"]</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitTpSl>> SetCollateralTpSlAsync(
            string symbol,
            decimal? takeProfitPrice = null,
            decimal? stopLossPrice = null,
            PositionSide? positionSide = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/api-reference/spot-trading/cancel-order" /><br />
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">["<c>order_id</c>"] The order id, either this or `clientOrderId` has to be provided</param>
        /// <param name="clientOrderId">["<c>client_order_id</c>"] The client order id, either this or `id` should be provided</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitOrder>> CancelOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-cancel-all" /><br />
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="orderProductTypes">The order product types to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The result of the cancel all orders operation</returns>
        Task<QueryResult> CancelAllOrdersAsync(string? symbol = null, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/websocket/order-management/order-modify" /><br />
        /// </para>
        /// </summary>
        /// <param name="orderId">["<c>order_id</c>"] The order id, either this or `clientOrderId` should be provided</param>
        /// <param name="clientOrderId">["<c>client_order_id</c>"] Client order id, either this or `orderId` should be provided</param>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="quantity">["<c>amount</c>"] New quantity</param>
        /// <param name="quoteQuantity">["<c>total</c>"] New quote quantity</param>
        /// <param name="price">["<c>price</c>"] New price</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] New trigger price</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitOrder>> EditOrderAsync(
           string symbol,
           long? orderId = null,
           string? clientOrderId = null,
           decimal? quantity = null,
           decimal? quoteQuantity = null,
           decimal? price = null,
           decimal? triggerPrice = null,
           CancellationToken ct = default);

        /// <summary>
        /// Get trades for a symbol
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/trades" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="fromId">Filter by returning results later than the trade with this id</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitSocketTrade[]>> GetTradeHistoryAsync(string symbol, int limit, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to public trade updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/trades" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default);
        
        /// <summary>
        /// Subscribe to public trade updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/trades" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to public book ticker updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/book-ticker" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitBookTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to public book ticker updates for all symbols
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/book-ticker" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(Action<DataEvent<WhiteBitBookTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the last price for a symbol
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/lastprice" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<decimal>> GetLastPriceAsync(string symbol, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/lastprice" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(string symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/lastprice" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the ticker for a symbol
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/market" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitSocketTicker>> GetTickerAsync(string symbol, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/market" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/market" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get klines for a symbol
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/kline" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="interval">Interval</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitKlineUpdate[]>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime startTime, DateTime endTime, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/kline" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="interval">The interval</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<WhiteBitKlineUpdate[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the ticker for a symbol
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="depth">Order book depth, max 100</param>
        /// <param name="priceInterval">0 - no interval default. Available values: "0.00000001", "0.0000001", "0.000001", "0.00001", "0.0001", "0.001", "0.01", "0.1"</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int depth, string? priceInterval = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="depth">The depth of the order book, 1, 5, 10, 20, 30, 50 or 100</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to premium index updates for all symbols
        /// <para><a href="https://docs.whitebit.com/websocket/market-streams/premium-index" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToPremiumIndexUpdatesAsync(Action<DataEvent<WhiteBitPremiumIndexUpdate[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get spot balances
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/balance-spot" /></para>
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<QueryResult<WhiteBitTradeBalance[]>> GetSpotBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Subscribe to spot balance updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/balance-spot" /></para>
        /// </summary>
        /// <param name="assets">Assets to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get margin balances
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/balance-margin" /></para>
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<QueryResult<WhiteBitMarginBalance[]>> GetMarginBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Subscribe to margin balance updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/balance-margin" /></para>
        /// </summary>
        /// <param name="assets">Assets to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToMarginBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<WhiteBitMarginBalance[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get open orders
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/orders-pending" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<QueryResult<WhiteBitOrders>> GetOpenOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to open order updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/orders-pending" /></para>
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="onOrderMessage">The event handler for handling an order update</param>
        /// <param name="onOtoOrdersMessage">The event handler for handling an OTO order update</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToOpenOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> onOrderMessage, Action<DataEvent<WhiteBitOtoOrderUpdate>>? onOtoOrdersMessage = null, CancellationToken ct = default);

        /// <summary>
        /// Get closed orders
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/orders-executed" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="orderTypes">Order types filter</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<QueryResult<WhiteBitClosedOrders>> GetClosedOrdersAsync(string symbol, IEnumerable<OrderType>? orderTypes = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to closed order updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/orders-executed" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="filter">Order type filter</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToClosedOrderUpdatesAsync(IEnumerable<string> symbols, ClosedOrderFilter filter, Action<DataEvent<WhiteBitClosedOrder[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get user trades
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<QueryResult<WhiteBitUserTrades>> GetUserTradesAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user trade updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/deals" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitUserTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to position updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/positions" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(Action<DataEvent<WhiteBitPositionsUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user position ADL quantile updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/adl-quantile" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToAdlQuantileUpdatesAsync(Action<DataEvent<WhiteBitAdlQuantileUpdate[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to borrow updates
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/borrows" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToBorrowUpdatesAsync(Action<DataEvent<WhiteBitBorrow>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user margin call and liquidation events
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/borrows-events" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToAccountBorrowEventUpdatesAsync(Action<DataEvent<WhiteBitAccountBorrowUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to position margin call and liquidation events
        /// <para><a href="https://docs.whitebit.com/websocket/account-streams/margin-positions-events" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToAccountMarginPositionEventUpdatesAsync(Action<DataEvent<WhiteBitAccountMarginPositionUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// [V1] Get the shared socket requests client. For new implementations prefer <see cref="SharedApi"/>
        /// </summary>
        public IWhiteBitSocketClientV4ApiShared SharedClient { get; }
        /// <summary>
        /// [V2] Gets the aggregate Shared API interface. Shared APIs provide a common,
        /// exchange-independent contract for accessing functionality across different
        /// exchange client libraries.
        /// </summary>
        public IWhiteBitSocketClientV4SharedApi SharedApi { get; }
    }
}

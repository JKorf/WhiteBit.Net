using CryptoExchange.Net.Objects;
using System;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Sockets;
using WhiteBit.Net.Objects.Models;
using System.Collections;
using System.Collections.Generic;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 streams
    /// </summary>
    public interface IWhiteBitSocketClientV4Api : ISocketApiClient, IDisposable
    {
        /// <summary>
        /// Get trades for a symbol
        /// <para><a href="https://docs.whitebit.com/public/websocket/#%EF%B8%8F-request-16" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="fromId">Filter by returning results later than the trade with this id</param>
        /// <param name="ct">Cancellation token</param>
        Task<CallResult<IEnumerable<WhiteBitSocketTrade>>> GetTradeHistoryAsync(string symbol, int limit, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to public trade updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#-update-events-5" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default);
        
        /// <summary>
        /// Subscribe to public trade updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#-update-events-5" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the last price for a symbol
        /// <para><a href="https://docs.whitebit.com/public/websocket/#%EF%B8%8F-request-7" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="ct">Cancellation token</param>
        Task<CallResult<decimal>> GetLastPriceAsync(string symbol, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#subscribe-1" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(string symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#subscribe-1" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the ticker for a symbol
        /// <para><a href="https://docs.whitebit.com/public/websocket/#%EF%B8%8F-request-10" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="ct">Cancellation token</param>
        Task<CallResult<WhiteBitSocketTicker>> GetTickerAsync(string symbol, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#subscribe-2" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);
        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#subscribe-2" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get klines for a symbol
        /// <para><a href="https://docs.whitebit.com/public/websocket/#%EF%B8%8F-request-4" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="interval">Interval</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="ct">Cancellation token</param>
        Task<CallResult<IEnumerable<WhiteBitKlineUpdate>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime startTime, DateTime endTime, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#subscribe" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="interval">The interval</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the ticker for a symbol
        /// <para><a href="https://docs.whitebit.com/public/websocket/#%EF%B8%8F-request-19" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="depth">Order book depth, max 100</param>
        /// <param name="priceInterval">0 - no interval default. Available values: "0.00000001", "0.0000001", "0.000001", "0.00001", "0.0001", "0.001", "0.01", "0.1"</param>
        /// <param name="ct">Cancellation token</param>
        Task<CallResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int depth, string? priceInterval = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.whitebit.com/public/websocket/#-update-events-6" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to subscribe</param>
        /// <param name="depth">The depth of the order book, 1, 5, 10, 20, 30, 50 or 100</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get spot balances
        /// <para><a href="https://docs.whitebit.com/private/websocket/#balance-spot" /></para>
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<CallResult<IEnumerable<WhiteBitTradeBalance>>> GetSpotBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Subscribe to spot balance updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#balance-spot" /></para>
        /// </summary>
        /// <param name="assets">Assets to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToSpotBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get margin balances
        /// <para><a href="https://docs.whitebit.com/private/websocket/#balance-margin" /></para>
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<CallResult<IEnumerable<WhiteBitMarginBalance>>> GetMarginBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Subscribe to margin balance updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#balance-margin" /></para>
        /// </summary>
        /// <param name="assets">Assets to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarginBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<IEnumerable<WhiteBitMarginBalance>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get open orders
        /// <para><a href="https://docs.whitebit.com/private/websocket/#orders-pending" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<CallResult<WhiteBitOrders>> GetOpenOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to open order updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#orders-pending" /></para>
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOpenOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get closed orders
        /// <para><a href="https://docs.whitebit.com/private/websocket/#orders-executed" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="orderTypes">Order types filter</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<CallResult<WhiteBitClosedOrders>> GetClosedOrdersAsync(string symbol, IEnumerable<OrderType>? orderTypes = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to closed order updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#orders-executed" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="filter">Order type filter</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToClosedOrderUpdatesAsync(IEnumerable<string> symbols, ClosedOrderFilter filter, Action<DataEvent<IEnumerable<WhiteBitClosedOrder>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get user trades
        /// <para><a href="https://docs.whitebit.com/private/websocket/#deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="limit">Max number of results, max 100</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        Task<CallResult<WhiteBitUserTrades>> GetUserTradesAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user trade updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#deals" /></para>
        /// </summary>
        /// <param name="symbols">Symbols to subscribe</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitUserTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to position updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#positions" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(Action<DataEvent<WhiteBitPositionsUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to borrow updates
        /// <para><a href="https://docs.whitebit.com/private/websocket/#borrows" /></para>
        /// </summary>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBorrowUpdatesAsync(Action<DataEvent<WhiteBitBorrow>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the shared socket requests client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IWhiteBitSocketClientV4ApiShared SharedClient { get; }
    }
}

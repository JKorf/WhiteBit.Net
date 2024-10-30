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

        Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(string symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);
        Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(IEnumerable<string> symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default);

        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default);

        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> onMessage, CancellationToken ct = default);

        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get the shared socket requests client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IWhiteBitSocketClientV4ApiShared SharedClient { get; }
    }
}

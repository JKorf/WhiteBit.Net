using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Objects.Options;
using WhiteBit.Net.Objects.Sockets.Subscriptions;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <summary>
    /// Client providing access to the WhiteBit V4 websocket Api
    /// </summary>
    internal partial class WhiteBitSocketClientV4Api : SocketApiClient, IWhiteBitSocketClientV4Api
    {
        #region fields
        private static readonly MessagePath _idPath = MessagePath.Get().Property("id");
        private static readonly MessagePath _methodPath = MessagePath.Get().Property("method");
        private static readonly MessagePath _index0SymbolPath = MessagePath.Get().Property("params").Index(0);
        private static readonly MessagePath _index7SymbolPath = MessagePath.Get().Property("params").Index(0).Index(7);
        private static readonly MessagePath _index2SymbolPath = MessagePath.Get().Property("params").Index(2);
        #endregion

        #region constructor/destructor

        /// <summary>
        /// ctor
        /// </summary>
        internal WhiteBitSocketClientV4Api(ILogger logger, WhiteBitSocketOptions options) :
            base(logger, options.Environment.SocketClientAddress!, options, options.V4Options)
        {
            RateLimiter = WhiteBitExchange.RateLimiter.WhiteBitSocket;
        }
        #endregion

        /// <inheritdoc />
        protected override IByteMessageAccessor CreateAccessor() => new SystemTextJsonByteMessageAccessor();
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new WhiteBitAuthenticationProvider(credentials);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToTradeUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitTradeUpdate>(_logger, "trades", symbols.ToArray(), x=> onMessage(x.WithSymbol(x.Data.Symbol)), false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(string symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToLastPriceUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitLastPriceUpdate>(_logger, "lastprice", symbols.ToArray(), x => onMessage(x.WithSymbol(x.Data.Symbol)), false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToTickerUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitTickerUpdate>(_logger, "market", symbols.ToArray(), x => onMessage(x.WithSymbol(x.Data.Symbol)), false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitKlineSubscription(_logger, symbol, interval, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> onMessage, CancellationToken ct = default)
        {
            depth.ValidateIntValues(nameof(depth), 1, 5, 10, 20, 30, 50, 100);

            var subscription = new WhiteBitBookSubscription(_logger, symbol, depth, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string? GetListenerIdentifier(IMessageAccessor message)
        {
            var id = message.GetValue<long?>(_idPath);
            if (id != null)
                return id.ToString();

            var method = message.GetValue<string>(_methodPath);
            if (method == null)
                return null;

            if (method.Equals("trades_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_index0SymbolPath);
            if (method.Equals("lastprice_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_index0SymbolPath);
            if (method.Equals("market_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_index0SymbolPath);
            if (method.Equals("candles_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_index7SymbolPath);
            if (method.Equals("depth_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_index2SymbolPath);

            return method;
        }

        /// <inheritdoc />
        protected override Query? GetAuthenticationRequest(SocketConnection connection) => null;

        /// <inheritdoc />
        public IWhiteBitSocketClientV4ApiShared SharedClient => this;

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverDate = null)
            => WhiteBitExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverDate);
    }
}

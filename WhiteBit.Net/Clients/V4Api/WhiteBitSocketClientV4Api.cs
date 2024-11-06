using System;
using System.Collections.Concurrent;
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
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Objects.Options;
using WhiteBit.Net.Objects.Sockets;
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
        private static readonly MessagePath _ordersSymbolPath = MessagePath.Get().Property("params").Index(1).Property("market");
        private static readonly MessagePath _orderExecutedSymbolPath = MessagePath.Get().Property("params").Property("market");

        /// <inheritdoc />
        public new WhiteBitSocketOptions ClientOptions => (WhiteBitSocketOptions)base.ClientOptions;

        private static readonly ConcurrentDictionary<string, CachedToken> _tokenCache = new();
        #endregion


        #region constructor/destructor

        /// <summary>
        /// ctor
        /// </summary>
        internal WhiteBitSocketClientV4Api(ILogger logger, WhiteBitSocketOptions options) :
            base(logger, options.Environment.SocketClientAddress!, options, options.V4Options)
        {
            RateLimiter = WhiteBitExchange.RateLimiter.WhiteBitSocket;
            AllowTopicsOnTheSameConnection = false;
        }
        #endregion

        /// <inheritdoc />
        protected override IByteMessageAccessor CreateAccessor() => new SystemTextJsonByteMessageAccessor();
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new WhiteBitAuthenticationProvider(credentials);

        #region Trades

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<WhiteBitSocketTrade>>> GetTradeHistoryAsync(string symbol, int limit, long? fromId = null, CancellationToken ct = default)
        {
            return await QueryAsync<IEnumerable<WhiteBitSocketTrade>>(
                "trades_request",
                false,
                ct,
                symbol,
                limit,
                fromId ?? 0).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToTradeUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTradeUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitTradeUpdate>(_logger, "trades", symbols.ToArray(), x=> onMessage(x.WithSymbol(x.Data.Symbol)), false, true);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #region Last Price

        /// <inheritdoc />
        public async Task<CallResult<decimal>> GetLastPriceAsync(string symbol, CancellationToken ct = default)
        {
            var result = await QueryAsync<decimal?>(
                "lastprice_request",
                false,
                ct,
                symbol).ConfigureAwait(false);
            return result.As(result.Data ?? default);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(string symbol, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToLastPriceUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLastPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitLastPriceUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitLastPriceUpdate>(_logger, "lastprice", symbols.ToArray(), x => onMessage(x.WithSymbol(x.Data.Symbol)), false, false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #region Ticker

        /// <inheritdoc />
        public async Task<CallResult<WhiteBitSocketTicker>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            return await QueryAsync<WhiteBitSocketTicker>(
                "market_request",
                false,
                ct,
                symbol,
                86400).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default)
            => await SubscribeToTickerUpdatesAsync([symbol], onMessage, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitTickerUpdate>(_logger, "market", symbols.ToArray(), x => onMessage(x.WithSymbol(x.Data.Symbol)), false, false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #region Kline

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<WhiteBitKlineUpdate>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime startTime, DateTime endTime, CancellationToken ct = default)
        {
            return await QueryAsync<IEnumerable<WhiteBitKlineUpdate>>(
                "candles_request",
                false,
                ct,
                symbol,
                DateTimeConverter.ConvertToSeconds(startTime),
                DateTimeConverter.ConvertToSeconds(endTime),
                (int)interval).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<WhiteBitKlineUpdate>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitKlineSubscription(_logger, symbol, interval, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #region Order book

        /// <inheritdoc />
        public async Task<CallResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int depth, string? priceInterval = null, CancellationToken ct = default)
        {
            depth.ValidateIntBetween(nameof(depth), 0, 100);

            return await QueryAsync<WhiteBitOrderBook>(
                "depth_request",
                false,
                ct,
                symbol,
                depth,
                priceInterval ?? "0").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, Action<DataEvent<WhiteBitBookUpdate>> onMessage, CancellationToken ct = default)
        {
            depth.ValidateIntValues(nameof(depth), 1, 5, 10, 20, 30, 50, 100);

            var subscription = new WhiteBitBookSubscription(_logger, symbol, depth, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #region Spot Balances

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<WhiteBitTradeBalance>>> GetSpotBalancesAsync(CancellationToken ct = default)
        {
            var result = await QueryAsync<Dictionary<string, WhiteBitTradeBalance>>(
                "balanceSpot_request",
                true,
                ct).ConfigureAwait(false);

            if (!result)
                return result.As<IEnumerable<WhiteBitTradeBalance>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitTradeBalance>>(result.Data.Values);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToSpotBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<Dictionary<string, WhiteBitTradeBalance>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSpotBalanceSubscription(_logger, assets.ToArray(), onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region Margin Balances

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<WhiteBitMarginBalance>>> GetMarginBalancesAsync(CancellationToken ct = default)
        {
            var result = await QueryAsync<Dictionary<string, WhiteBitMarginBalance>>(
                "balanceMargin_request",
                true,
                ct).ConfigureAwait(false);

            if (!result)
                return result.As<IEnumerable<WhiteBitMarginBalance>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitMarginBalance>>(result.Data.Values);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarginBalanceUpdatesAsync(IEnumerable<string> assets, Action<DataEvent<IEnumerable<WhiteBitMarginBalance>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitMarginBalanceSubscription(_logger, assets.ToArray(), onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region Open Orders

        /// <inheritdoc />
        public async Task<CallResult<WhiteBitOrders>> GetOpenOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            return await QueryAsync<WhiteBitOrders>(
                "ordersPending_request",
                true,
                ct,
                symbol,
                limit ?? 0,
                offset ?? 100).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOpenOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitOrderUpdate>(_logger, "ordersPending", symbols.ToArray(), onMessage, true, false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region Open Orders

        /// <inheritdoc />
        public async Task<CallResult<WhiteBitClosedOrders>> GetClosedOrdersAsync(string symbol, IEnumerable<OrderType>? orderTypes, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            return await QueryAsync<WhiteBitClosedOrders>(
                "ordersExecuted_request",
                true,
                ct,
                new {
                    market = symbol,
                    order_types = orderTypes.Select(x => (int)x).ToArray()
                },
                limit ?? 0,
                offset ?? 100).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToClosedOrderUpdatesAsync(IEnumerable<string> symbols, ClosedOrderFilter filter, Action<DataEvent<IEnumerable<WhiteBitClosedOrder>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitClosedOrderSubscription(_logger, symbols.ToArray(), (int)filter, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region User Trades

        /// <inheritdoc />
        public async Task<CallResult<WhiteBitUserTrades>> GetUserTradesAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            return await QueryAsync<WhiteBitUserTrades>(
                "deals_request",
                true,
                ct,
                symbol,
                limit ?? 0,
                offset ?? 100).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<WhiteBitUserTradeUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitUserTradeSubscription(_logger, symbols.ToArray(), onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region Positions

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(Action<DataEvent<WhiteBitPositionsUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitPositionsUpdate>(_logger, "positionsMargin", [], onMessage, true, true);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #region Positions

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBorrowUpdatesAsync(Action<DataEvent<WhiteBitBorrow>> onMessage, CancellationToken ct = default)
        {
            var subscription = new WhiteBitSubscription<WhiteBitBorrow>(_logger, "borrowsMargin", [], onMessage, true, true);
            return await SubscribeAsync(BaseAddress.AppendPath("ws"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

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

            if (method.Equals("ordersPending_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_ordersSymbolPath);
            if (method.Equals("ordersExecuted_update", StringComparison.Ordinal))
                return method + "." + message.GetValue<string>(_orderExecutedSymbolPath);

            return method;
        }

        private async Task<CallResult<T>> QueryAsync<T>(string method, bool auth, CancellationToken ct, params object[] parameters)
        {
            var query = new WhiteBitQuery<T>(new WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = method,
                Request = parameters
            }, auth);

            var result = await QueryAsync(BaseAddress.AppendPath("ws"), query, ct).ConfigureAwait(false);
            return result.As<T>(result.Data == null ? default: result.Data.Result);
        }

        /// <inheritdoc />
        protected override async Task<Query?> GetAuthenticationRequestAsync(SocketConnection connection)
        {
            var token = await GetTokenAsync().ConfigureAwait(false);

            return new WhiteBitQuery<WhiteBitSubscribeResponse>(new WhiteBitSocketRequest
            {
                Id = ExchangeHelpers.NextId(),
                Method = "authorize",
                Request = [
                    token.Data,
                    "public"
                    ]
            }, false);
        }

        private async Task<CallResult<string>> GetTokenAsync()
        {
            var apiCredentials = ApiOptions.ApiCredentials ?? ClientOptions.ApiCredentials;
            if (apiCredentials == null)
                return new CallResult<string>(new NoApiCredentialsError());

            if (_tokenCache.TryGetValue(apiCredentials.Key, out var token) && token.Expire > DateTime.UtcNow)
                return new CallResult<string>(token.Token);

            _logger.LogDebug("Requesting websocket token");
            var restClient = new WhiteBitRestClient(x =>
            {
                x.ApiCredentials = apiCredentials;
                x.Environment = ClientOptions.Environment;
            });

            var result = await ((WhiteBitRestClientV4ApiAccount)restClient.V4Api.Account).GetWebsocketTokenAsync().ConfigureAwait(false);
            if (result)
                _tokenCache[apiCredentials.Key] = new CachedToken { Token = result.Data, Expire = DateTime.UtcNow.AddSeconds(60) };
            else
                _logger.LogWarning("Failed to retrieve websocket token: {Error}", result.Error);

            return result.As<string>(result.Data);
        }

        private class CachedToken
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expire { get; set; }
        }

        /// <inheritdoc />
        public IWhiteBitSocketClientV4ApiShared SharedClient => this;

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverDate = null)
            => WhiteBitExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverDate);
    }
}

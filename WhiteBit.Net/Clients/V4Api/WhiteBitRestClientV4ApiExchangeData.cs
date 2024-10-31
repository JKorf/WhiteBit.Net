using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting.Guards;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc />
    internal class WhiteBitRestClientV4ApiExchangeData : IWhiteBitRestClientV4ApiExchangeData
    {
        private readonly WhiteBitRestClientV4Api _baseClient;
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();

        internal WhiteBitRestClientV4ApiExchangeData(ILogger logger, WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Server Time

        /// <inheritdoc />
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/time", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitTime>(request, null, ct).ConfigureAwait(false);
            return result.As(result.Data?.Timestamp ?? default);
        }

        #endregion

        #region Get Symbols

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitSymbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/markets", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitSymbol>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get System Status

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitSystemStatus>> GetSystemStatusAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/platform/status", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitSystemStatus>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Tickers

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitTicker>>> GetTickersAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/ticker", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitTicker>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitTicker>>(default);

            foreach (var item in result.Data)
                item.Value.Symbol = item.Key;

            return result.As<IEnumerable<WhiteBitTicker>>(result.Data.Select(x => x.Value).ToArray());
        }

        #endregion

        #region Get Assets

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitAsset>>> GetAssetsAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/assets", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitAsset>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitAsset>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitAsset>>(result.Data?.Values);
        }

        #endregion

        #region Get Order Book

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, int? mergeLevel = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("level", mergeLevel);
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/api/v4/public/orderbook/{symbol}", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(600, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrderBook>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Recent Trades

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitTrade>>> GetRecentTradesAsync(string symbol, OrderSide? side = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalEnum("type", side);
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/api/v4/public/trades/{symbol}", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitTrade>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Deposit Withdrawal Info

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitDepositWithdraw>> GetDepositWithdrawalInfoAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/fee", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositWithdraw>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Colleteral Symbols

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetCollateralSymbolsAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/collateral/markets", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitResponse<IEnumerable<string>>>(request, parameters, ct).ConfigureAwait(false);
            return result.As<IEnumerable<string>>(result.Data?.Result);
        }

        #endregion

        #region Get Futures Symbols

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitFuturesSymbol>>> GetFuturesSymbolsAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/public/futures", WhiteBitExchange.RateLimiter.WhiteBit, 1, false,
                limitGuard: new SingleLimitGuard(2000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitResponse<IEnumerable<WhiteBitFuturesSymbol>>>(request, parameters, ct).ConfigureAwait(false);
            return result.As<IEnumerable<WhiteBitFuturesSymbol>>(result.Data?.Result);
        }

        #endregion

    }
}

using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Clients.V4Api;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;
using System;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc />
    internal class WhiteBitRestClientV4ApiAccount : IWhiteBitRestClientV4ApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;

        internal WhiteBitRestClientV4ApiAccount(WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Main Balances

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitMainBalance>>> GetMainBalancesAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitMainBalance>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitMainBalance>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitMainBalance>>(result.Data?.Values);
        }

        #endregion

        #region Get Deposit Address

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("ticker", asset);
            parameters.AddOptional("network", network);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/address", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitDepositAddressInfo>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Trading Balances

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitTradeBalance>>> GetTradingBalancesAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/trade-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitTradeBalance>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitTradeBalance>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitTradeBalance>>(result.Data?.Values);
        }

        #endregion

        #region Get Convert Estimate

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitConvertEstimate>> GetConvertEstimateAsync(string fromAsset, string toAsset, decimal quantity, string fromOrToQuantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("from", fromAsset);
            parameters.Add("to", toAsset);
            parameters.AddString("amount", quantity);
            parameters.Add("direction", fromOrToQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/estimate", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitConvertEstimate>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Confirm Convert

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitConvertResult>> ConfirmConvertAsync(string estimateId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("quoteId", estimateId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/confirm", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitConvertResult>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Convert History

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitConvertHistory>> GetConvertHistoryAsync(string? fromAsset = null, string? toAsset = null, string? quoteId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("fromTicker", fromAsset);
            parameters.AddOptional("toTicker", toAsset);
            parameters.AddOptional("quoteId", quoteId);
            parameters.AddOptionalMillisecondsString("from", startTime);
            parameters.AddOptionalMillisecondsString("to", endTime);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitConvertHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

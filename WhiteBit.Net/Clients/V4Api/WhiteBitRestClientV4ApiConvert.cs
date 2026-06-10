using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Models;
using System;
using CryptoExchange.Net.RateLimiting.Guards;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc />
    internal class WhiteBitRestClientV4ApiConvert : IWhiteBitRestClientV4ApiConvert
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;

        internal WhiteBitRestClientV4ApiConvert(WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Convert Estimate

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitConvertEstimate>> GetConvertEstimateAsync(string fromAsset, string toAsset, decimal quantity, string fromOrToQuantity, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("from", fromAsset);
            parameters.Add("to", toAsset);
            parameters.Add("amount", quantity);
            parameters.Add("direction", fromOrToQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/convert/estimate", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitConvertEstimate>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Confirm Convert

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitConvertResult>> ConfirmConvertAsync(string estimateId, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("quoteId", estimateId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/convert/confirm", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitConvertResult>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Convert History

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitConvertHistory>> GetConvertHistoryAsync(string? fromAsset = null, string? toAsset = null, string? quoteId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("fromTicker", fromAsset);
            parameters.Add("toTicker", toAsset);
            parameters.Add("quoteId", quoteId);
            parameters.Add("from", startTime);
            parameters.Add("to", endTime);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/convert/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitConvertHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Clients.V4Api;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;
using System;
using CryptoExchange.Net.RateLimiting.Guards;
using System.Linq;
using System.IO;
using WhiteBit.Net.Enums;

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
        public async Task<WebCallResult<WhiteBitConvertEstimate>> GetConvertEstimateAsync(string fromAsset, string toAsset, decimal quantity, string fromOrToQuantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("from", fromAsset);
            parameters.Add("to", toAsset);
            parameters.AddString("amount", quantity);
            parameters.Add("direction", fromOrToQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/estimate", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/confirm", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/convert/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitConvertHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

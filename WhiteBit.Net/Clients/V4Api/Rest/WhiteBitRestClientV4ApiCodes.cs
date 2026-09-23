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
    internal class WhiteBitRestClientV4ApiCodes : IWhiteBitRestClientV4ApiCodes
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;

        internal WhiteBitRestClientV4ApiCodes(WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
        }

        #region Create Code

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitCode>> CreateCodeAsync(string asset, decimal quantity, string? passphrase = null, string? description = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            parameters.Add("amount", quantity);
            parameters.Add("passphrase", passphrase);
            parameters.Add("description", description);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/codes", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCode>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Apply Code

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitCodeResult>> ApplyCodeAsync(string code, string? passphrase = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("code", code);
            parameters.Add("passphrase", passphrase);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/codes/apply", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(60, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCodeResult>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Created Codes

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitGeneratedCodes>> GetCreatedCodesAsync(int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v4/main-account/codes/my", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitGeneratedCodes>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Code History

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitGeneratedCodes>> GetCodeHistoryAsync(int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/codes/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitGeneratedCodes>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

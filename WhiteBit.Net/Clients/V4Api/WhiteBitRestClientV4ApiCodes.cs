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
        public async Task<WebCallResult<WhiteBitCode>> CreateCodeAsync(string asset, decimal quantity, string? passphrase = null, string? description = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("ticker", asset);
            parameters.AddString("amount", quantity);
            parameters.AddOptional("passphrase", passphrase);
            parameters.AddOptional("description", description);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/codes", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCode>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Apply Code

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitCodeResult>> ApplyCodeAsync(string code, string? passphrase = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("code", code);
            parameters.AddOptional("passphrase", passphrase);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/codes/apply", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(60, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCodeResult>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Created Codes

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitGeneratedCodes>> GetCreatedCodesAsync(int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/api/v4/main-account/codes/my", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitGeneratedCodes>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Code History

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitGeneratedCodes>> GetCodeHistoryAsync(int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/codes/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitGeneratedCodes>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

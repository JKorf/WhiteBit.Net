using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Clients.V4Api;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;

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
        public async Task<WebCallResult<Dictionary<string, WhiteBitMainBalance>>> GetMainBalancesAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitMainBalance>>(request, parameters, ct).ConfigureAwait(false);
            return result;
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

    }
}

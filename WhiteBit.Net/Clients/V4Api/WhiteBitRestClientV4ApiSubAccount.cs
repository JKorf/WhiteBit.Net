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
    internal class WhiteBitRestClientV4ApiSubAccount : IWhiteBitRestClientV4ApiSubAccount
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;

        internal WhiteBitRestClientV4ApiSubAccount(WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
        }

        #region Create Sub Account

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitSubAccount>> CreateSubAccountAsync(string alias, string? email = null, bool? shareKyc = null, string? spotEnabled = null, string? collateralEnabled = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("alias", alias);
            parameters.AddOptional("email", email);
            parameters.AddOptional("shareKyc", shareKyc);
            var permissions = new ParameterCollection();
            permissions.AddOptional("spotEnabled", spotEnabled);
            permissions.AddOptional("collateralEnabled", collateralEnabled);
            parameters.Add("permissions", permissions);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/create", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitSubAccount>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Delete Sub Account

        /// <inheritdoc />
        public async Task<WebCallResult> DeleteSubAccountAsync(string id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", id);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/delete", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Edit Sub Account

        /// <inheritdoc />
        public async Task<WebCallResult> EditSubAccountAsync(string id, string alias, string spotEnabled, string collateralEnabled, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", id);
            parameters.Add("alias", alias);
            var permissions = new ParameterCollection();
            permissions.Add("spotEnabled", spotEnabled);
            permissions.Add("collateralEnabled", collateralEnabled);
            parameters.Add("permissions", permissions);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/edit", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Sub Accounts

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitSubAccounts>> GetSubAccountsAsync(string? search = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("search", search);
            parameters.AddOptional("", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/list", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitSubAccounts>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Subaccount Transfer

        /// <inheritdoc />
        public async Task<WebCallResult> SubaccountTransferAsync(string subaccountId, SubTransferDirection direction, string asset, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", subaccountId);
            parameters.AddEnum("direction", direction);
            parameters.Add("ticker", asset);
            parameters.AddString("amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/transfer", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding)); ;
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Block Subaccount

        /// <inheritdoc />
        public async Task<WebCallResult> BlockSubaccountAsync(string id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", id);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/block", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Block Subaccount

        /// <inheritdoc />
        public async Task<WebCallResult> UnblockSubaccountAsync(string id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", id);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/unblock", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Subaccount Balances

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitSubBalances>>> GetSubaccountBalancesAsync(string id, string? asset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("id", id);
            parameters.AddOptional("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/balances", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitSubBalances>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitSubBalances>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitSubBalances>>(result.Data.Select(x => x.Value).ToArray());
        }

        #endregion

        #region Get Subaccount Transfer History

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitSubaccountTransferHistory>> GetSubaccountTransferHistoryAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/sub-account/transfer/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitSubaccountTransferHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

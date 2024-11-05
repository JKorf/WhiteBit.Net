using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/address", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding)); 
            var result = await _baseClient.SendAsync<WhiteBitDepositAddressInfo>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Spot Balances

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitTradeBalance>>> GetSpotBalancesAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/trade-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitTradeBalance>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<WhiteBitTradeBalance>>(default);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return result.As<IEnumerable<WhiteBitTradeBalance>>(result.Data?.Values);
        }

        #endregion

        #region Get Fiat Deposit Address

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitDepositUrl>> GetFiatDepositAddressAsync(string asset, string provider, decimal quantity, string clientOrderId, string? successLink = null, string? failureLink = null, string? returnLink = null, string? customerFirstName = null, string? customerLastName = null, string? customerEmail = null, string? customerAddressLine1 = null, string? customerAddressLine2 = null, string? customerCity = null, string? customerZipCode = null, string? customerCountryCode = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("ticker", asset);
            parameters.Add("provider", provider);
            parameters.AddString("amount", quantity);
            parameters.Add("uniqueId", clientOrderId);
            parameters.AddOptional("successLink", successLink);
            parameters.AddOptional("failureLink", failureLink);
            parameters.AddOptional("returnLink", returnLink);
            var customer = new ParameterCollection();
            customer.AddOptional("firstName", customerFirstName);
            customer.AddOptional("lastName", customerLastName);
            customer.AddOptional("email", customerEmail);
            var address = new ParameterCollection();
            address.AddOptional("addressLine1", customerAddressLine1);
            address.AddOptional("addressLine2", customerAddressLine2);
            address.AddOptional("city", customerCity);
            address.AddOptional("zipCode", customerZipCode);
            address.AddOptional("countryCode", customerCountryCode);
            if (address.Any())
                customer.Add("address", address);
            if (customer.Any())
            parameters.Add("customer", customer);

            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/fiat-deposit-url", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositUrl>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Withdraw

        /// <inheritdoc />
        public async Task<WebCallResult> WithdrawAsync(string asset, decimal quantity, string address, string uniqueId, bool deductFeeFromOutput, string? memo = null, string? provider = null, string? network = null, bool? partialEnable = null, string? beneficiaryFirstName = null, string? beneficiaryLastName = null, string? beneficiaryTin = null, string? beneficiaryPhone = null, string? beneficiaryEmail = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("ticker", asset);
            parameters.AddString("amount", quantity);
            parameters.Add("address", address);
            parameters.Add("uniqueId", uniqueId);
            parameters.AddOptional("memo", memo);
            parameters.AddOptional("provider", provider);
            parameters.AddOptional("network", network);
            parameters.AddOptional("partialEnable", partialEnable);
            var beneficiary = new ParameterCollection();
            beneficiary.AddOptional("firstName", beneficiaryFirstName);
            beneficiary.AddOptional("lastName", beneficiaryLastName);
            beneficiary.AddOptional("tin", beneficiaryTin);
            beneficiary.AddOptional("phone", beneficiaryPhone);
            beneficiary.AddOptional("email", beneficiaryEmail);
            if (beneficiary.Any())
                parameters.Add("beneficiary", beneficiary);

            var path = "/api/v4/main-account/withdraw";
            if (!deductFeeFromOutput)
                path = "/api/v4/main-account/withdraw-pay";

            var request = _definitions.GetOrCreate(HttpMethod.Post, path, WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Transfer

        /// <inheritdoc />
        public async Task<WebCallResult> TransferAsync(AccountType fromAccount, AccountType toAccount, string asset, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("from", fromAccount);
            parameters.AddEnum("to", toAccount);
            parameters.Add("ticker", asset);
            parameters.AddString("amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/transfer", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Deposit Withdrawal History

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitDepositWithdrawals>> GetDepositWithdrawalHistoryAsync(TransactionType? type = null, string? asset = null, string? address = null, string? memo = null, string? addresses = null, string? uniqueId = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalEnumAsInt("transactionMethod", type);
            parameters.AddOptional("ticker", asset);
            parameters.AddOptional("address", address);
            parameters.AddOptional("memo", memo);
            parameters.AddOptional("addresses", addresses);
            parameters.AddOptional("uniqueId", uniqueId);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(200, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositWithdrawals>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Create Deposit Address

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitDepositAddressInfo>> CreateDepositAddressAsync(string asset, string? network = null, string? addressType = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("ticker", asset);
            parameters.AddOptional("network", network);
            parameters.AddOptional("type", addressType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/create-new-address", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositAddressInfo>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Deposit Withdrawal Settings

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitDepositWithdrawalSetting>>> GetDepositWithdrawalSettingsAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/main-account/fee", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitDepositWithdrawalSetting>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Mining Reward History

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitMiningRewards>> GetMiningRewardHistoryAsync(string? accountName = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("account", accountName);
            parameters.AddOptionalMilliseconds("from", startTime);
            parameters.AddOptionalMilliseconds("to", endTime);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/mining/rewards", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitMiningRewards>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Balances

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, decimal>>> GetCollateralBalancesAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, decimal>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Balance Summary

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitCollateralSummary>>> GetCollateralBalanceSummaryAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/balance-summary", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitCollateralSummary>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Account Summary

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitCollateralAccountSummary>> GetCollateralAccountSummaryAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/summary", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCollateralAccountSummary>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Set Account Leverage

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitLeverage>> SetAccountLeverageAsync(int leverage, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("leverage", leverage);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/leverage", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitLeverage>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        internal async Task<WebCallResult<string>> GetWebsocketTokenAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/profile/websocket_token", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitToken>(request, null, ct).ConfigureAwait(false);
            return result.As<string>(result.Data?.Token);
        }
    }
}

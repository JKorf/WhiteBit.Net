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
        public async Task<HttpResult<WhiteBitMainBalance[]>> GetMainBalancesAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitMainBalance>>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<WhiteBitMainBalance[]>(result);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return HttpResult.Ok(result, result.Data.Values.ToArray());
        }

        #endregion

        #region Get Deposit Address

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            parameters.Add("network", network);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/address", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding)); 
            var result = await _baseClient.SendAsync<WhiteBitDepositAddressInfo>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Spot Balances

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitTradeBalance[]>> GetSpotBalancesAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/trade-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitTradeBalance>>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<WhiteBitTradeBalance[]>(result);

            foreach (var item in result.Data)
                item.Value.Asset = item.Key;

            return HttpResult.Ok(result, result.Data.Values.ToArray());
        }

        #endregion

        #region Get Fiat Deposit Address

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitDepositUrl>> GetFiatDepositAddressAsync(string asset, string provider, decimal quantity, string clientOrderId, string? successLink = null, string? failureLink = null, string? returnLink = null, string? customerFirstName = null, string? customerLastName = null, string? customerEmail = null, string? customerAddressLine1 = null, string? customerAddressLine2 = null, string? customerCity = null, string? customerZipCode = null, string? customerCountryCode = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            parameters.Add("provider", provider);
            parameters.Add("amount", quantity);
            parameters.Add("uniqueId", clientOrderId);
            parameters.Add("successLink", successLink);
            parameters.Add("failureLink", failureLink);
            parameters.Add("returnLink", returnLink);
            var customer = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            customer.Add("firstName", customerFirstName);
            customer.Add("lastName", customerLastName);
            customer.Add("email", customerEmail);
            var address = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            address.Add("addressLine1", customerAddressLine1);
            address.Add("addressLine2", customerAddressLine2);
            address.Add("city", customerCity);
            address.Add("zipCode", customerZipCode);
            address.Add("countryCode", customerCountryCode);
            if (address.Any())
                customer.Add("address", address);
            if (customer.Any())
            parameters.Add("customer", customer);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/fiat-deposit-url", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositUrl>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Withdraw

        /// <inheritdoc />
        public async Task<HttpResult> WithdrawAsync(string asset, decimal quantity, string address, string uniqueId, bool deductFeeFromOutput, string? memo = null, string? provider = null, string? network = null, bool? partialEnable = null, string? beneficiaryFirstName = null, string? beneficiaryLastName = null, string? beneficiaryTin = null, string? beneficiaryPhone = null, string? beneficiaryEmail = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            parameters.Add("amount", quantity);
            parameters.Add("address", address);
            parameters.Add("uniqueId", uniqueId);
            parameters.Add("memo", memo);
            parameters.Add("provider", provider);
            parameters.Add("network", network);
            parameters.Add("partialEnable", partialEnable);
            var beneficiary = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            beneficiary.Add("firstName", beneficiaryFirstName);
            beneficiary.Add("lastName", beneficiaryLastName);
            beneficiary.Add("tin", beneficiaryTin);
            beneficiary.Add("phone", beneficiaryPhone);
            beneficiary.Add("email", beneficiaryEmail);
            if (beneficiary.Any())
                parameters.Add("beneficiary", beneficiary);

            var path = "/api/v4/main-account/withdraw";
            if (!deductFeeFromOutput)
                path = "/api/v4/main-account/withdraw-pay";

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, path, WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Transfer

        /// <inheritdoc />
        public async Task<HttpResult> TransferAsync(AccountType fromAccount, AccountType toAccount, string asset, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("from", fromAccount);
            parameters.Add("to", toAccount);
            parameters.Add("ticker", asset);
            parameters.Add("amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/transfer", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Deposit Withdrawal History

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitDepositWithdrawals>> GetDepositWithdrawalHistoryAsync(TransactionType? type = null, string? asset = null, string? address = null, string? memo = null, string? addresses = null, string? uniqueId = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("transactionMethod", type, EnumSerialization.Number);
            parameters.Add("ticker", asset);
            parameters.Add("address", address);
            parameters.Add("memo", memo);
            parameters.Add("addresses", addresses);
            parameters.Add("uniqueId", uniqueId);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(200, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositWithdrawals>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Create Deposit Address

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitDepositAddressInfo>> CreateDepositAddressAsync(string asset, string? network = null, string? addressType = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            parameters.Add("network", network);
            parameters.Add("type", addressType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/create-new-address", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositAddressInfo>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Deposit Withdrawal Settings

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitDepositWithdrawalSetting[]>> GetDepositWithdrawalSettingsAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/main-account/fee", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitDepositWithdrawalSetting[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Mining Reward History

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitMiningRewards>> GetMiningRewardHistoryAsync(string? accountName = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("account", accountName);
            parameters.Add("from", startTime);
            parameters.Add("to", endTime);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/mining/rewards", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitMiningRewards>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Balances

        /// <inheritdoc />
        public async Task<HttpResult<Dictionary<string, decimal>>> GetCollateralBalancesAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("ticker", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/balance", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, decimal>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Balance Summary

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitCollateralSummary[]>> GetCollateralBalanceSummaryAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/balance-summary", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitCollateralSummary[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Collateral Account Summary

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitCollateralAccountSummary>> GetCollateralAccountSummaryAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/summary", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCollateralAccountSummary>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Account Funding History

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitAccountFundingHistories>> GetAccountFundingHistoryAsync(
            string symbol,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            int? offset = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };

            parameters.Add("startDate", startTime);
            parameters.Add("endDate", endTime);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, 
                _baseClient.BaseAddress,
                "/api/v4/collateral-account/funding-history", 
                WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<WhiteBitAccountFundingHistories>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Set Account Leverage

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitLeverage>> SetAccountLeverageAsync(int leverage, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("leverage", leverage);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/leverage", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitLeverage>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        // Doesn't seem to respect the provided market parameter
        //#region Get Trading Fee

        ///// <inheritdoc />
        //public async Task<HttpResult<WhiteBitTradingFee>> GetTradingFeeAsync(string symbol, CancellationToken ct = default)
        //{
        //    var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
        //    parameters.Add("market", symbol);
        //    var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/market/fee", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
        //    return await _baseClient.SendAsync<WhiteBitTradingFee>(request, parameters, null, ct).ConfigureAwait(false);
        //}

        //#endregion

        #region Get Trading Fees

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitTradingFees>> GetTradingFeesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/market/fee", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            return await _baseClient.SendAsync<WhiteBitTradingFees>(request, null, ct).ConfigureAwait(false);
        }

        #endregion

        #region Get Hedge Mode

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitHedgeMode>> GetHedgeModeAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/hedge-mode", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitHedgeMode>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Set Hedge Mode

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitHedgeMode>> SetHedgeModeAsync(bool enableHedgeMode, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("hedgeMode", enableHedgeMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/collateral-account/hedge-mode/update", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitHedgeMode>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion


        internal async Task<HttpResult<string>> GetWebsocketTokenAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/profile/websocket_token", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitToken>(request, null, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<string>(result);

            return HttpResult.Ok(result, result.Data.Token);
        }
    }
}

using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;
using System;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface IWhiteBitRestClientV4ApiAccount
    {
        /// <summary>
        /// Get main account balances
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#main-balance" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/balance
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitMainBalance[]>> GetMainBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get deposit address
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#get-cryptocurrency-deposit-address" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/address
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] The asset</param>
        /// <param name="network">["<c>network</c>"] Network to use</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default);

        /// <summary>
        /// Get spot trading balances
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#trading-balance" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/trade-account/balance
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] Filter by asset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTradeBalance[]>> GetSpotBalancesAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get fiat deposit url, note that his endpoint is not available by default and has to be activated for you by WhiteBit support
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#get-fiat-deposit-address" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/fiat-deposit-url
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] The asset</param>
        /// <param name="provider">["<c>provider</c>"] The provider</param>
        /// <param name="quantity">["<c>amount</c>"] The quantity</param>
        /// <param name="clientOrderId">["<c>uniqueId</c>"] Client order id</param>
        /// <param name="successLink">["<c>successLink</c>"] Success redirect link</param>
        /// <param name="failureLink">["<c>failureLink</c>"] Failure redirect link</param>
        /// <param name="returnLink">["<c>returnLink</c>"] Redirect link when user cancels</param>
        /// <param name="customerFirstName">["<c>customer.firstName</c>"] Customer first name</param>
        /// <param name="customerLastName">["<c>customer.lastName</c>"] Customer last name</param>
        /// <param name="customerEmail">["<c>customer.email</c>"] Customer email</param>
        /// <param name="customerAddressLine1">["<c>customer.address.addressLine1</c>"] Customer address line 1</param>
        /// <param name="customerAddressLine2">["<c>customer.address.addressLine2</c>"] Customer address line 2</param>
        /// <param name="customerCity">["<c>customer.address.city</c>"] Customer city</param>
        /// <param name="customerZipCode">["<c>customer.address.zipCode</c>"] Customer zip code</param>
        /// <param name="customerCountryCode">["<c>customer.address.countryCode</c>"] Customer country code</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositUrl>> GetFiatDepositAddressAsync(string asset, string provider, decimal quantity, string clientOrderId, string? successLink = null, string? failureLink = null, string? returnLink = null, string? customerFirstName = null, string? customerLastName = null, string? customerEmail = null, string? customerAddressLine1 = null, string? customerAddressLine2 = null, string? customerCity = null, string? customerZipCode = null, string? customerCountryCode = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw fiat or crypto
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#create-withdraw-request" /><br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#create-withdraw-request-with-the-specific-withdraw-amount-fee-is-not-included" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/withdraw<br />
        /// POST /api/v4/main-account/withdraw-pay
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] The asset, for example `ETH`</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity to withdraw</param>
        /// <param name="address">["<c>address</c>"] Target address (wallet address for cryptocurrencies, identifier/card number for fiat currencies)</param>
        /// <param name="uniqueId">["<c>uniqueId</c>"] Unique id</param>
        /// <param name="deductFeeFromOutput">True: fee is deducted from the output amount, false: fee is deducted from the send amount</param>
        /// <param name="memo">["<c>memo</c>"] Memo</param>
        /// <param name="provider">["<c>provider</c>"] Provider name for fiat withdrawal</param>
        /// <param name="network">["<c>network</c>"] Network for crypto withdrawal</param>
        /// <param name="partialEnable">["<c>partialEnable</c>"] Partial enable for fiat withdrawal</param>
        /// <param name="beneficiaryFirstName">["<c>beneficiary.firstName</c>"] Beneficiary first name for fiat withdrawal</param>
        /// <param name="beneficiaryLastName">["<c>beneficiary.lastName</c>"] Beneficiary last name for fiat withdrawal</param>
        /// <param name="beneficiaryTin">["<c>beneficiary.tin</c>"] Beneficiary TAX payer number</param>
        /// <param name="beneficiaryPhone">["<c>beneficiary.phone</c>"] Beneficiary phone number</param>
        /// <param name="beneficiaryEmail">["<c>beneficiary.email</c>"] Beneficiary email</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> WithdrawAsync(string asset, decimal quantity, string address, string uniqueId, bool deductFeeFromOutput, string? memo = null, string? provider = null, string? network = null, bool? partialEnable = null, string? beneficiaryFirstName = null, string? beneficiaryLastName = null, string? beneficiaryTin = null, string? beneficiaryPhone = null, string? beneficiaryEmail = null, CancellationToken ct = default);
        
        /// <summary>
        /// Transfer between accounts
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#transfer-between-balances" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/transfer
        /// </para>
        /// </summary>
        /// <param name="fromAccount">["<c>from</c>"] From account</param>
        /// <param name="toAccount">["<c>to</c>"] To account</param>
        /// <param name="asset">["<c>ticker</c>"] The asset, for example `ETH`</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity to transfer</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> TransferAsync(AccountType fromAccount, AccountType toAccount, string asset, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Get deposit/withdrawal history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#get-depositwithdraw-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/history
        /// </para>
        /// </summary>
        /// <param name="type">["<c>transactionMethod</c>"] Filter by type</param>
        /// <param name="asset">["<c>ticker</c>"] Filter by asset</param>
        /// <param name="address">["<c>address</c>"] Filter by address</param>
        /// <param name="memo">["<c>memo</c>"] Filter by memo</param>
        /// <param name="addresses">["<c>addresses</c>"] Filter by multiple addresses</param>
        /// <param name="uniqueId">["<c>uniqueId</c>"] Filter by uniqueId</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositWithdrawals>> GetDepositWithdrawalHistoryAsync(TransactionType? type = null, string? asset = null, string? address = null, string? memo = null, string? addresses = null, string? uniqueId = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Create a new deposit address, note that his endpoint is not available by default and has to be activated for you by WhiteBit support
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#create-new-address-for-deposit" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/create-new-address
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] Asset name</param>
        /// <param name="network">["<c>network</c>"] Asset network</param>
        /// <param name="addressType">["<c>type</c>"] Address type for BTC/LTC: p2sh-segwit, bech32</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> CreateDepositAddressAsync(string asset, string? network = null, string? addressType = null, CancellationToken ct = default);

        /// <summary>
        /// Get deposit/withdrawal settings
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#fees" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/fee
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositWithdrawalSetting[]>> GetDepositWithdrawalSettingsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mining rewards history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#get-rewards" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/mining/rewards
        /// </para>
        /// </summary>
        /// <param name="accountName">["<c>account</c>"] Filter by account name</param>
        /// <param name="startTime">["<c>from</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>to</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitMiningRewards>> GetMiningRewardHistoryAsync(string? accountName = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get collateral balances
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-balance" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/balance
        /// </para>
        /// </summary>
        /// <param name="asset">["<c>ticker</c>"] Filter by asset, for example `ETH`</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<Dictionary<string, decimal>>> GetCollateralBalancesAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get collateral balance summary
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-balance-summary" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/balance-summary
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitCollateralSummary[]>> GetCollateralBalanceSummaryAsync(CancellationToken ct = default);
        
        /// <summary>
        /// Get collateral account summary
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-summary" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/summary
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitCollateralAccountSummary>> GetCollateralAccountSummaryAsync(CancellationToken ct = default);

        /// <summary>
        /// Asynchronously retrieves the account funding history for the specified trading pair.
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#funding-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/funding-history
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The trading pair symbol for which to retrieve the funding history, such as "BTC_PERP". This value cannot be
        /// null or empty.</param>
        /// <param name="startTime">["<c>startDate</c>"] The optional UTC start time for the funding history retrieval. If specified, only records after this time
        /// are returned.</param>
        /// <param name="endTime">["<c>endDate</c>"] The optional UTC end time for the funding history retrieval. If specified, only records before this time are
        /// returned.</param>
        /// <param name="limit">["<c>limit</c>"] The optional maximum number of records to return. If not specified, a default limit is applied.</param>
        /// <param name="offset">["<c>offset</c>"] The optional offset for pagination. Specifies the number of records to skip before returning results.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a WebCallResult with the funding
        /// history records for the specified account and symbol.</returns>
        Task<WebCallResult<WhiteBitAccountFundingHistories>> GetAccountFundingHistoryAsync(
            string symbol,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            int? offset = null,
            CancellationToken ct = default);

        /// <summary>
        /// Set leverage for the entire account, both spot margin and futures. Spot margin leverage is capped at x20.
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#change-collateral-account-leverage" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/leverage
        /// </para>
        /// </summary>
        /// <param name="leverage">["<c>leverage</c>"] New leverage setting</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitLeverage>> SetAccountLeverageAsync(int leverage, CancellationToken ct = default);

        /// <summary>
        /// Get the users trading fees
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-all-market-fees" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/market/fee
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTradingFees>> GetTradingFeesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get current hedge mode status
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-hedge-mode" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/hedge-mode
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitHedgeMode>> GetHedgeModeAsync(CancellationToken ct = default);

        /// <summary>
        /// Set hedge mode
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-hedge-mode" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/hedge-mode/update
        /// </para>
        /// </summary>
        /// <param name="enableHedgeMode">["<c>hedgeMode</c>"] Hedge mode enabled or not</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitHedgeMode>> SetHedgeModeAsync(bool enableHedgeMode, CancellationToken ct = default);

    }
}

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
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#main-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitMainBalance>>> GetMainBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get deposit address
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-cryptocurrency-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="network">Network to use</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default);

        /// <summary>
        /// Get spot trading balances
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#trading-balance" /></para>
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitTradeBalance>>> GetSpotBalancesAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get fiat deposit url, note that his endpoint is not available by default and has to be activated for you by WhiteBit support
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-fiat-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="provider">The provider</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="successLink">Success redirect link</param>
        /// <param name="failureLink">Failure redirect link</param>
        /// <param name="returnLink">Redirect link when user cancels</param>
        /// <param name="customerFirstName">Customer first name</param>
        /// <param name="customerLastName">Customer last name</param>
        /// <param name="customerEmail">Customer email</param>
        /// <param name="customerAddressLine1">Customer address line 1</param>
        /// <param name="customerAddressLine2">Customer address line 2</param>
        /// <param name="customerCity">Customer city</param>
        /// <param name="customerZipCode">Customer zip code</param>
        /// <param name="customerCountryCode">Customer country code</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositUrl>> GetFiatDepositAddressAsync(string asset, string provider, decimal quantity, string clientOrderId, string? successLink = null, string? failureLink = null, string? returnLink = null, string? customerFirstName = null, string? customerLastName = null, string? customerEmail = null, string? customerAddressLine1 = null, string? customerAddressLine2 = null, string? customerCity = null, string? customerZipCode = null, string? customerCountryCode = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw fiat or crypto
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#create-withdraw-request" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#create-withdraw-request-with-the-specific-withdraw-amount-fee-is-not-included" /></para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="quantity">Quantity to withdraw</param>
        /// <param name="address">Target address (wallet address for cryptocurrencies, identifier/card number for fiat currencies)</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="deductFeeFromOutput">True: fee is deducted from the output amount, false: fee is deducted from the send amount</param>
        /// <param name="memo">Memo</param>
        /// <param name="provider">Provider name for fiat withdrawal</param>
        /// <param name="network">Network for crypto withdrawal</param>
        /// <param name="partialEnable">Partial enable for fiat withdrawal</param>
        /// <param name="beneficiaryFirstName">Beneficiary first name for fiat withdrawal</param>
        /// <param name="beneficiaryLastName">Beneficiary last name for fiat withdrawal</param>
        /// <param name="beneficiaryTin">Beneficiary TAX payer number</param>
        /// <param name="beneficiaryPhone">Beneficiary phone number</param>
        /// <param name="beneficiaryEmail">Beneficiary email</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> WithdrawAsync(string asset, decimal quantity, string address, string uniqueId, bool deductFeeFromOutput, string? memo = null, string? provider = null, string? network = null, bool? partialEnable = null, string? beneficiaryFirstName = null, string? beneficiaryLastName = null, string? beneficiaryTin = null, string? beneficiaryPhone = null, string? beneficiaryEmail = null, CancellationToken ct = default);
        
        /// <summary>
        /// Transfer between accounts
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#transfer-between-balances" /></para>
        /// </summary>
        /// <param name="fromAccount">From account</param>
        /// <param name="toAccount">To account</param>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="quantity">Quantity to transfer</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> TransferAsync(AccountType fromAccount, AccountType toAccount, string asset, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Get deposit/withdrawal history
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-depositwithdraw-history" /></para>
        /// </summary>
        /// <param name="type">Filter by type</param>
        /// <param name="asset">Filter by asset</param>
        /// <param name="address">Filter by address</param>
        /// <param name="memo">Filter by memo</param>
        /// <param name="addresses">Filter by multiple addresses</param>
        /// <param name="uniqueId">Filter by uniqueId</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositWithdrawals>> GetDepositWithdrawalHistoryAsync(TransactionType? type = null, string? asset = null, string? address = null, string? memo = null, string? addresses = null, string? uniqueId = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Create a new deposit address, note that his endpoint is not available by default and has to be activated for you by WhiteBit support
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#create-new-address-for-deposit" /></para>
        /// </summary>
        /// <param name="asset">Asset name</param>
        /// <param name="network">Asset network</param>
        /// <param name="addressType">Address type for BTC/LTC: p2sh-segwit, bech32</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> CreateDepositAddressAsync(string asset, string? network = null, string? addressType = null, CancellationToken ct = default);

        /// <summary>
        /// Get deposit/withdrawal settings
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#fees" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitDepositWithdrawalSetting>>> GetDepositWithdrawalSettingsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mining rewards history
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-rewards" /></para>
        /// </summary>
        /// <param name="accountName">Filter by account name</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitMiningRewards>> GetMiningRewardHistoryAsync(string? accountName = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get collateral balances
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-balance" /></para>
        /// </summary>
        /// <param name="asset">Filter by asset, for example `ETH`</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<Dictionary<string, decimal>>> GetCollateralBalancesAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get collateral balance summary
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-balance-summary" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitCollateralSummary>>> GetCollateralBalanceSummaryAsync(CancellationToken ct = default);
        
        /// <summary>
        /// Get collateral account summary
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-account-summary" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitCollateralAccountSummary>> GetCollateralAccountSummaryAsync(CancellationToken ct = default);

        /// <summary>
        /// Set leverage for the entire account, both spot margin and futures. Spot margin leverage is capped at x20.
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#change-collateral-account-leverage" /></para>
        /// </summary>
        /// <param name="leverage">New leverage setting</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitLeverage>> SetAccountLeverageAsync(int leverage, CancellationToken ct = default);

    }
}

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
    /// WhiteBit V4 sub account endpoints
    /// </summary>
    public interface IWhiteBitRestClientV4ApiSubAccount
    {
        /// <summary>
        /// Create a new sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#create-sub-account" /></para>
        /// </summary>
        /// <param name="alias">Account name</param>
        /// <param name="email">If provided an invitation email will be send</param>
        /// <param name="shareKyc">If KYC is shared with main account</param>
        /// <param name="spotEnabled">Whether spot trading is enabled</param>
        /// <param name="collateralEnabled">Whether collateral trading is enabled</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubAccount>> CreateSubAccountAsync(string alias, string? email = null, bool? shareKyc = null, string? spotEnabled = null, string? collateralEnabled = null, CancellationToken ct = default);
        
        /// <summary>
        /// Delete a sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#delete-sub-account" /></para>
        /// </summary>
        /// <param name="id">Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> DeleteSubAccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Edit a sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#edit-sub-account" /></para>
        /// </summary>
        /// <param name="id">Sub account id</param>
        /// <param name="alias">Sub account alias</param>
        /// <param name="spotEnabled">Spot trading enabled</param>
        /// <param name="collateralEnabled">Collateral trading enabled</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> EditSubAccountAsync(string id, string alias, string spotEnabled, string collateralEnabled, CancellationToken ct = default);

        /// <summary>
        /// Get sub account list
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#edit-sub-account" /></para>
        /// </summary>
        /// <param name="search">Search string</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubAccounts>> GetSubAccountsAsync(string? search = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Transfer to or from sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#sub-account-transfer" /></para>
        /// </summary>
        /// <param name="subaccountId">Sub account id</param>
        /// <param name="direction">Transfer direction</param>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> SubaccountTransferAsync(string subaccountId, SubTransferDirection direction, string asset, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Block a sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#block-sub-account" /></para>
        /// </summary>
        /// <param name="id">Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> BlockSubaccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Unblock a sub account
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#unblock-sub-account" /></para>
        /// </summary>
        /// <param name="id">Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> UnblockSubaccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Get sub account balances
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#sub-account-balances" /></para>
        /// </summary>
        /// <param name="id">Sub account id</param>
        /// <param name="asset">Asset name</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitSubBalances>>> GetSubaccountBalancesAsync(string id, string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get sub account transfer history
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-sub-account-transfer-history" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubaccountTransferHistory>> GetSubaccountTransferHistoryAsync(CancellationToken ct = default);

    }
}

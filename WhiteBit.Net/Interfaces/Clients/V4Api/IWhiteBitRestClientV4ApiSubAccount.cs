using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Objects.Models;
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
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#create-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/create
        /// </para>
        /// </summary>
        /// <param name="alias">["<c>alias</c>"] Account name</param>
        /// <param name="email">["<c>email</c>"] If provided an invitation email will be send</param>
        /// <param name="shareKyc">["<c>shareKyc</c>"] If KYC is shared with main account</param>
        /// <param name="spotEnabled">["<c>permissions.spotEnabled</c>"] Whether spot trading is enabled</param>
        /// <param name="collateralEnabled">["<c>permissions.collateralEnabled</c>"] Whether collateral trading is enabled</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubAccount>> CreateSubAccountAsync(string alias, string? email = null, bool? shareKyc = null, string? spotEnabled = null, string? collateralEnabled = null, CancellationToken ct = default);
        
        /// <summary>
        /// Delete a sub account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#delete-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/delete
        /// </para>
        /// </summary>
        /// <param name="id">["<c>id</c>"] Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> DeleteSubAccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Edit a sub account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#edit-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/edit
        /// </para>
        /// </summary>
        /// <param name="id">["<c>id</c>"] Sub account id</param>
        /// <param name="alias">["<c>alias</c>"] Sub account alias</param>
        /// <param name="spotEnabled">["<c>permissions.spotEnabled</c>"] Spot trading enabled</param>
        /// <param name="collateralEnabled">["<c>permissions.collateralEnabled</c>"] Collateral trading enabled</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> EditSubAccountAsync(string id, string alias, string spotEnabled, string collateralEnabled, CancellationToken ct = default);

        /// <summary>
        /// Get sub account list
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#edit-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/list
        /// </para>
        /// </summary>
        /// <param name="search">["<c>search</c>"] Search string</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubAccounts>> GetSubAccountsAsync(string? search = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Transfer to or from sub account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#sub-account-transfer" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/transfer
        /// </para>
        /// </summary>
        /// <param name="subaccountId">["<c>id</c>"] Sub account id</param>
        /// <param name="direction">["<c>direction</c>"] Transfer direction</param>
        /// <param name="asset">["<c>ticker</c>"] The asset, for example `ETH`</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> SubaccountTransferAsync(string subaccountId, SubTransferDirection direction, string asset, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Block a sub account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#block-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/block
        /// </para>
        /// </summary>
        /// <param name="id">["<c>id</c>"] Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> BlockSubaccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Unblock a sub account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#unblock-sub-account" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/unblock
        /// </para>
        /// </summary>
        /// <param name="id">["<c>id</c>"] Sub account id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> UnblockSubaccountAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Get sub account balances
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#sub-account-balances" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/balances
        /// </para>
        /// </summary>
        /// <param name="id">["<c>id</c>"] Sub account id</param>
        /// <param name="asset">["<c>ticker</c>"] Asset name</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubBalances[]>> GetSubaccountBalancesAsync(string id, string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get sub account transfer history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-main-v4/#get-sub-account-transfer-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/sub-account/transfer/history
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSubaccountTransferHistory>> GetSubaccountTransferHistoryAsync(CancellationToken ct = default);

    }
}

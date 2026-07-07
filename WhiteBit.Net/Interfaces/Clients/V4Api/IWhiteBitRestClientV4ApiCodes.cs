using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 code endpoints
    /// </summary>
    public interface IWhiteBitRestClientV4ApiCodes
    {
        /// <summary>
        /// Create a new WhiteBit Code for funds transfer
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/api-reference/account-wallet/create-code" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/codes
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="passphrase">Passphrase for applying the code</param>
        /// <param name="description">Description</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<WhiteBitCode>> CreateCodeAsync(string asset, decimal quantity, string? passphrase = null, string? description = null, CancellationToken ct = default);

        /// <summary>
        /// Apply a WhiteBit code
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/api-reference/account-wallet/apply-code" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/codes/apply
        /// </para>
        /// </summary>
        /// <param name="code">The WhiteBit code</param>
        /// <param name="passphrase">Code passphrase</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<WhiteBitCodeResult>> ApplyCodeAsync(string code, string? passphrase = null, CancellationToken ct = default);

        /// <summary>
        /// Get generated code history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/api-reference/account-wallet/get-my-codes" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/main-account/codes/my
        /// </para>
        /// </summary>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<WhiteBitGeneratedCodes>> GetCreatedCodesAsync(int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get account code history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/api-reference/account-wallet/get-codes-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/main-account/codes/history
        /// </para>
        /// </summary>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<WhiteBitGeneratedCodes>> GetCodeHistoryAsync(int? limit = null, int? offset = null, CancellationToken ct = default);

    }
}

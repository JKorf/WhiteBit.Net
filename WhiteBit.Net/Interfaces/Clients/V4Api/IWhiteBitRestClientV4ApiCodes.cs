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
    /// WhiteBit V4 code endpoints
    /// </summary>
    public interface IWhiteBitRestClientV4ApiCodes
    {
        /// <summary>
        /// Create a new WhiteBit Code for funds transfer
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#create-code" /></para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="passphrase">Passphrase for applying the code</param>
        /// <param name="description">Description</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitCode>> CreateCodeAsync(string asset, decimal quantity, string? passphrase = null, string? description = null, CancellationToken ct = default);

        /// <summary>
        /// Apply a WhiteBit code
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#apply-code" /></para>
        /// </summary>
        /// <param name="code">The WhiteBit code</param>
        /// <param name="passphrase">Code passphrase</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitCodeResult>> ApplyCodeAsync(string code, string? passphrase = null, CancellationToken ct = default);

        /// <summary>
        /// Get generated code history
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-my-codes" /></para>
        /// </summary>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitGeneratedCodes>> GetCreatedCodesAsync(int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get account code history
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#get-codes-history" /></para>
        /// </summary>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitGeneratedCodes>> GetCodeHistoryAsync(int? limit = null, int? offset = null, CancellationToken ct = default);

    }
}

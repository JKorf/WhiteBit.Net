using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Objects.Models;
using System;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 convert endpoints
    /// </summary>
    public interface IWhiteBitRestClientV4ApiConvert
    {
        /// <summary>
        /// Get convert estimate
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#convert-estimate" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/convert/estimate
        /// </para>
        /// </summary>
        /// <param name="fromAsset">["<c>from</c>"] From asset</param>
        /// <param name="toAsset">["<c>to</c>"] To asset</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity</param>
        /// <param name="fromOrToQuantity">["<c>direction</c>"] Whether the quantity is specified in the fromAsset or toAsset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertEstimate>> GetConvertEstimateAsync(string fromAsset, string toAsset, decimal quantity, string fromOrToQuantity, CancellationToken ct = default);

        /// <summary>
        /// Accept/confirm a convert estimate
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#convert-confirm" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/convert/confirm
        /// </para>
        /// </summary>
        /// <param name="estimateId">["<c>quoteId</c>"] Quote/estimate id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertResult>> ConfirmConvertAsync(string estimateId, CancellationToken ct = default);

        /// <summary>
        /// Get convert history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#convert-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/convert/history
        /// </para>
        /// </summary>
        /// <param name="fromAsset">["<c>fromTicker</c>"] Filter by from asset</param>
        /// <param name="toAsset">["<c>toTicker</c>"] Filter by to asset</param>
        /// <param name="quoteId">["<c>quoteId</c>"] Filter by quote id</param>
        /// <param name="startTime">["<c>from</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>to</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertHistory>> GetConvertHistoryAsync(string? fromAsset = null, string? toAsset = null, string? quoteId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

    }
}

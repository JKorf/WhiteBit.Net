using System.Threading.Tasks;
using System.Threading;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using WhiteBit.Net.Objects.Models;
using System;

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
        /// <para><a href="https://docs.whitebit.com/private/http-main-v4/#main-balance" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="network">Network to use</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositAddressInfo>> GetDepositAddressAsync(string asset, string? network = null, CancellationToken ct = default);
       
        /// <summary>
        /// Get trading balances
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#trading-balance" /></para>
        /// </summary>
        /// <param name="asset">Filter by asset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitTradeBalance>>> GetTradingBalancesAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get convert estimate
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#convert" /></para>
        /// </summary>
        /// <param name="fromAsset">From asset</param>
        /// <param name="toAsset">To asset</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="fromOrToQuantity">Whether the quantity is specified in the fromAsset or toAsset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertEstimate>> GetConvertEstimateAsync(string fromAsset, string toAsset, decimal quantity, string fromOrToQuantity, CancellationToken ct = default);

        /// <summary>
        /// Accept/confirm a convert estimate
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#convert" /></para>
        /// </summary>
        /// <param name="estimateId">Quote/estimate id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertResult>> ConfirmConvertAsync(string estimateId, CancellationToken ct = default);

        /// <summary>
        /// Get convert history
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#convert" /></para>
        /// </summary>
        /// <param name="fromAsset">Filter by from asset</param>
        /// <param name="toAsset">Filter by to asset</param>
        /// <param name="quoteId">Filter by quote id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConvertHistory>> GetConvertHistoryAsync(string? fromAsset = null, string? toAsset = null, string? quoteId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

    }
}

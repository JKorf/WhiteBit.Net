using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface IWhiteBitRestClientV4ApiExchangeData
    {
        /// <summary>
        /// Get the current server time
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#server-time" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        /// <summary>
        /// Get symbols list
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#market-info" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSymbol[]>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get system/platform status
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#maintenance-status" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSystemStatus>> GetSystemStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Get tickers
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#market-activity" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTicker[]>> GetTickersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get asset information
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#asset-status-list" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitAsset[]>> GetAssetsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the order book for a symbol
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#orderbook" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="limit">The order book depth, max 100</param>
        /// <param name="mergeLevel">Aggregation level</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, int? mergeLevel = null, CancellationToken ct = default);

        /// <summary>
        /// Get the most recent 100 trades
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#recent-trades" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by trade side</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTrade[]>> GetRecentTradesAsync(string symbol, OrderSide? side = null, CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal/deposit limits and fees
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#fee" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositWithdraw>> GetDepositWithdrawalInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get collateral symbols
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#collateral-markets-list" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<string[]>> GetCollateralSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get futures symbols
        /// <para><a href="https://docs.whitebit.com/public/http-v4/#available-futures-markets-list" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitFuturesSymbol[]>> GetFuturesSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get funding rate history
        /// </summary>
        /// <param name="symbol">Symbol name, for example `BTC_PERP`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitFundingHistory[]>> GetFundingHistoryAsync(
            string symbol,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            int? offset = null,
            CancellationToken ct = default);
    }
}

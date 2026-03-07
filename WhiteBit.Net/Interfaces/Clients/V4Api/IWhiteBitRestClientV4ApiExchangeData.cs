using System;
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
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#server-time" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/time
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        /// <summary>
        /// Get symbols list
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#market-info" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/markets
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSymbol[]>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get system/platform status
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#maintenance-status" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/platform/status
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitSystemStatus>> GetSystemStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Get tickers
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#market-activity" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/ticker
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTicker[]>> GetTickersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get asset information
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#asset-status-list" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/assets
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitAsset[]>> GetAssetsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the order book for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#orderbook" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/orderbook/{symbol}
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="limit">["<c>limit</c>"] The order book depth, max 100</param>
        /// <param name="mergeLevel">["<c>level</c>"] Aggregation level</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, int? mergeLevel = null, CancellationToken ct = default);

        /// <summary>
        /// Get the most recent 100 trades
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#recent-trades" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/trades/{symbol}
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETHUSDT`</param>
        /// <param name="side">["<c>type</c>"] Filter by trade side</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitTrade[]>> GetRecentTradesAsync(string symbol, OrderSide? side = null, CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal/deposit limits and fees
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#fee" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/fee
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitDepositWithdraw>> GetDepositWithdrawalInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get collateral symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#collateral-markets-list" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/collateral/markets
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<string[]>> GetCollateralSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get futures symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/public/http-v4/#available-futures-markets-list" /><br />
        /// Endpoint:<br />
        /// GET /api/v4/public/futures
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitFuturesSymbol[]>> GetFuturesSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get funding rate history
        /// </summary>
        /// <param name="symbol">Symbol name, for example `BTC_PERP`</param>
        /// <param name="startTime">["<c>startDate</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>endDate</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
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

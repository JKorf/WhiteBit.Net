using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using System.Collections.Generic;
using System;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 collateral trading endpoints (Futures and Spot Margin), placing and managing orders.
    /// </summary>
    public interface IWhiteBitRestClientV4ApiCollateralTrading
    {
        /// <summary>
        /// Place a new order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-limit-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-market-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-stop-limit-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-trigger-market-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity of the order in base asset</param>
        /// <param name="price"></param>
        /// <param name="postOnly"></param>
        /// <param name="immediateOrCancel"></param>
        /// <param name="stopLossPrice"></param>
        /// <param name="takeProfitPrice"></param>
        /// <param name="bboRole"></param>
        /// <param name="triggerPrice"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<WebCallResult<WhiteBitCollateralOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            NewOrderType type,
            decimal? quantity = null,
            decimal? price = null,
            bool? postOnly = null,
            bool? immediateOrCancel = null,
            int? bboRole = null,
            decimal? stopLossPrice = null,
            decimal? takeProfitPrice = null,
            decimal? triggerPrice = null,
            string? clientOrderId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get open positions
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#open-positions" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitPosition>>> GetOpenPositionsAsync(string? symbol = null, CancellationToken ct = default);

        /// <summary>
        /// Get position history
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#positions-history" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">Filter by position id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitPositionHistory>>> GetPositionHistoryAsync(string? symbol = null, long? positionId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get open conditional orders
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#query-unexecutedactive-conditional-orders" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConditionalOrdersResult>> GetOpenConditionalOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Place a new OCO order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-collateral-oco-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_PERP`</param>
        /// <param name="orderSide">Order side</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Limit price</param>
        /// <param name="triggerPrice">Trigger price</param>
        /// <param name="stopLimitPrice">Stop limit price</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOcoOrder>> PlaceOcoOrderAsync(string symbol, OrderSide orderSide, decimal quantity, decimal price, decimal triggerPrice, decimal stopLimitPrice, string? clientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel an OCO order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-oco-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOcoOrder>> CancelOcoOrderAsync(string symbol, long orderId, CancellationToken ct = default);
        
        /// <summary>
        /// Cancel a conditional order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-conditional-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelConditionalOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an OTO order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-conditional-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelOTOOrderAsync(string symbol, long orderId, CancellationToken ct = default);

    }
}

using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
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
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-limit-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-market-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-stop-limit-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#collateral-trigger-market-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/collateral/limit<br />
        /// POST /api/v4/order/collateral/market<br />
        /// POST /api/v4/order/collateral/stop-limit<br />
        /// POST /api/v4/order/collateral/trigger-market
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity of the order in base asset</param>
        /// <param name="price">["<c>price</c>"] Limit price</param>
        /// <param name="postOnly">["<c>postOnly</c>"] Post only flag</param>
        /// <param name="immediateOrCancel">["<c>ioc</c>"] Immediate or cancel flag</param>
        /// <param name="stopLossPrice">["<c>stopLoss</c>"] Stop loss price</param>
        /// <param name="takeProfitPrice">["<c>takeProfit</c>"] Take profit price</param>
        /// <param name="bboRole">["<c>bboRole</c>"] BBO role</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] Trigger price</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Client order id</param>
        /// <param name="stpMode">["<c>stp</c>"] Self trade prevention mode</param>
        /// <param name="positionSide">["<c>positionSide</c>"] Position side, required when in hedge mode</param>
        /// <param name="ct">Cancellation token</param>
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
            SelfTradePreventionMode? stpMode = null,
            PositionSide? positionSide = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get open positions
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#open-positions" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/positions/open
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitPosition[]>> GetOpenPositionsAsync(string? symbol = null, CancellationToken ct = default);

        /// <summary>
        /// Get position history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#positions-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/collateral-account/positions/history
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">["<c>positionId</c>"] Filter by position id</param>
        /// <param name="startTime">["<c>startDate</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>endDate</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitPositionHistory[]>> GetPositionHistoryAsync(string? symbol = null, long? positionId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get open conditional orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-unexecutedactive-conditional-orders" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/conditional-orders
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitConditionalOrdersResult>> GetOpenConditionalOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Place a new OCO order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-collateral-oco-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/collateral/oco
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_PERP`</param>
        /// <param name="orderSide">["<c>side</c>"] Order side</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity</param>
        /// <param name="price">["<c>price</c>"] Limit price</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] Trigger price</param>
        /// <param name="stopLimitPrice">["<c>stop_limit_price</c>"] Stop limit price</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Client order id</param>
        /// <param name="positionSide">["<c>positionSide</c>"] Position side, required when in hedge mode</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOcoOrder>> PlaceOcoOrderAsync(
            string symbol,
            OrderSide orderSide,
            decimal quantity,
            decimal price,
            decimal triggerPrice,
            decimal stopLimitPrice,
            string? clientOrderId = null,
            PositionSide? positionSide = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel an OCO order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-oco-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/oco-cancel
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">["<c>orderId</c>"] Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOcoOrder>> CancelOcoOrderAsync(string symbol, long orderId, CancellationToken ct = default);
        
        /// <summary>
        /// Cancel a conditional order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-conditional-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/conditional-cancel
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">["<c>id</c>"] Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelConditionalOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an OTO order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-conditional-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/oto-cancel
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_PERP`</param>
        /// <param name="orderId">["<c>otoId</c>"] Order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelOTOOrderAsync(string symbol, long orderId, CancellationToken ct = default);

    }
}

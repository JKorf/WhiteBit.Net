using System;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using System.Collections.Generic;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 trading endpoints, placing and managing orders.
    /// </summary>
    public interface IWhiteBitRestClientV4ApiTrading
    {
        /// <summary>
        /// Place a new order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-limit-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-market-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-buy-stock-market-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-stop-limit-order" /><br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#create-stop-market-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/new<br />
        /// POST /api/v4/order/market<br />
        /// POST /api/v4/order/stock_market<br />
        /// POST /api/v4/order/stop_limit<br />
        /// POST /api/v4/order/stop_market
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETH_USDT`</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity of the order in base asset</param>
        /// <param name="quoteQuantity">["<c>amount</c>"] Quantity of the order in quote asset, only supported for market buy orders</param>
        /// <param name="price">["<c>price</c>"] Limit price</param>
        /// <param name="postOnly">["<c>postOnly</c>"] Post only flag</param>
        /// <param name="immediateOrCancel">["<c>ioc</c>"] Immediate or cancel flag</param>
        /// <param name="bboRole">["<c>bboRole</c>"] BBO role</param>
        /// <param name="triggerPrice">["<c>activation_price</c>"] Trigger price</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Client order id</param>
        /// <param name="stpMode">["<c>stp</c>"] Self trade prevention mode</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<WhiteBitOrder>> PlaceSpotOrderAsync(
            string symbol,
            OrderSide side,
            NewOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            bool? postOnly = null,
            bool? immediateOrCancel = null,
            int? bboRole = null,
            decimal? triggerPrice = null,
            string? clientOrderId = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple order in a single call. Only supports limit orders. Check the response data for individual order placement results
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#bulk-limit-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/bulk
        /// </para>
        /// </summary>
        /// <param name="requests">["<c>orders</c>"] Orders to place, max 20</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<CallResult<WhiteBitOrderResponse>[]>> PlaceSpotMultipleOrdersAsync(
            IEnumerable<WhiteBitOrderRequest> requests,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/cancel
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">["<c>orderId</c>"] The order id, either this or `clientOrderId` has to be provided</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] The client order id, either this or `id` should be provided</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrder>> CancelOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-all-orders" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/cancel/all
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol</param>
        /// <param name="orderProductTypes">["<c>type</c>"] Filter by order product types</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelAllOrdersAsync(string? symbol = null, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default);

        /// <summary>
        /// Get list of currently open orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-unexecutedactive-orders" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/orders
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">["<c>orderId</c>"] Filter by order id</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Filter by client order id</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrder[]>> GetOpenOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get list of closed orders per symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-orders" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/trade-account/order/history
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">["<c>orderId</c>"] Filter by order id</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Filter by client order id</param>
        /// <param name="status">["<c>status</c>"] Filter by order status</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="startTime">["<c>startDate</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>endDate</c>"] Filter by end time</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<Dictionary<string, WhiteBitClosedOrder[]>>> GetClosedOrdersAsync(
            string? symbol = null,
            long? orderId = null, 
            string? clientOrderId = null,
            ClosedOrderStatus? status = null,
            int? limit = null,
            int? offset = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get user trade history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-order-history" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/trade-account/executed-history
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETH_USDT`</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Filter by client order id</param>
        /// <param name="startTime">["<c>startTime</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>endTime</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitUserTrade[]>> GetUserTradesAsync(string? symbol = null, string? clientOrderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get trades for a specific order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-order-deals" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/trade-account/order
        /// </para>
        /// </summary>
        /// <param name="orderId">["<c>orderId</c>"] The order id</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results</param>
        /// <param name="offset">["<c>offset</c>"] Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitUserTrade[]>> GetOrderTradesAsync(long orderId, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#modify-order" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/modify
        /// </para>
        /// </summary>
        /// <param name="orderId">["<c>orderId</c>"] The order id, either this or `clientOrderId` should be provided</param>
        /// <param name="clientOrderId">["<c>clientOrderId</c>"] Client order id, either this or `orderId` should be provided</param>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="quantity">["<c>amount</c>"] New quantity</param>
        /// <param name="quoteQuantity">["<c>total</c>"] New quote quantity</param>
        /// <param name="price">["<c>price</c>"] New price</param>
        /// <param name="triggerPrice">["<c>activationPrice</c>"] New trigger price</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrder>> EditOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, decimal? quantity = null, decimal? quoteQuantity = null, decimal? price = null, decimal? triggerPrice = null, CancellationToken ct = default);

        /// <summary>
        /// Set a kill switch. After the specified timeout all order fitting the parameters will be canceled unless the kill switch endpoint is called again to extend or cancel the timeout
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#sync-kill-switch-timer" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/kill-switch
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETH_USDT`</param>
        /// <param name="timeout">["<c>timeout</c>"] The timeout in seconds, or 0 to cancel the kill switch</param>
        /// <param name="symbolProductTypes">["<c>types</c>"] Symbol product types</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> SetKillSwitchAsync(string symbol, int timeout, IEnumerable<OrderProductType>? symbolProductTypes = null, CancellationToken ct = default);

        /// <summary>
        /// Get the status of enabled kill switches
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.whitebit.com/private/http-trade-v4/#status-kill-switch-timer" /><br />
        /// Endpoint:<br />
        /// POST /api/v4/order/kill-switch/status
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitKillSwitch[]>> GetKillSwitchStatusAsync(string? symbol = null, CancellationToken ct = default);
    }
}

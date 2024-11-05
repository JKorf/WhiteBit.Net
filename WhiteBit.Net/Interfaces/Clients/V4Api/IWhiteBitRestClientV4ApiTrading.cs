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
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-limit-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-market-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-buy-stock-market-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-stop-limit-order" /></para>
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#create-stop-market-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity of the order in base asset</param>
        /// <param name="quoteQuantity">Quantity of the order in quote asset, only supported for market buy orders</param>
        /// <param name="price"></param>
        /// <param name="postOnly"></param>
        /// <param name="immediateOrCancel"></param>
        /// <param name="bboRole"></param>
        /// <param name="triggerPrice"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="ct"></param>
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
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple order in a single call. Only supports limit orders. Check the response data for individual order placement results
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#bulk-limit-order" /></para>
        /// </summary>
        /// <param name="requests">Orders to place</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitOrderResponse>>> PlaceSpotMultipleOrdersAsync(
            IEnumerable<WhiteBitOrderRequest> requests,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel an order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="id">The order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrder>> CancelOrderAsync(string symbol, long id, CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#cancel-all-orders" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="orderProductTypes">Filter by order product types</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> CancelAllOrdersAsync(string? symbol = null, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default);

        /// <summary>
        /// Get list of currently open orders
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#query-unexecutedactive-orders" /></para>
        /// </summary>
        /// <param name="symbol">Filter symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">Filter by order id</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitOrder>>> GetOpenOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get list of closed orders per symbol
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-orders" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETH_USDT`</param>
        /// <param name="orderId">Filter by order id</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="status">Filter by order status</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result ofset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<Dictionary<string, IEnumerable<WhiteBitClosedOrder>>>> GetClosedOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, ClosedOrderStatus? status = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get user trade history
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-order-history" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETH_USDT`</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitUserTrade>>> GetUserTradesAsync(string? symbol = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Get trades for a specific order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#query-executed-order-deals" /></para>
        /// </summary>
        /// <param name="orderId">The order id</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="offset">Result offset</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitUserTrade>>> GetOrderTradesAsync(long orderId, int? limit = null, int? offset = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an order
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#modify-order" /></para>
        /// </summary>
        /// <param name="orderId">The order id</param>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="quoteQuantity">New quote quantity</param>
        /// <param name="price">New price</param>
        /// <param name="triggerPrice">New trigger price</param>
        /// <param name="clientOrderId">New client order id</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<WhiteBitOrder>> EditOrderAsync(string symbol, long orderId, decimal? quantity = null, decimal? quoteQuantity = null, decimal? price = null, decimal? triggerPrice = null, string? clientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Set a kill switch. After the specified timeout all order fitting the parameters will be canceled unless the kill switch endpoint is called again to extend or cancel the timeout
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#sync-kill-switch-timer" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETH_USDT`</param>
        /// <param name="timeout">The timeout in seconds, or 0 to cancel the kill switch</param>
        /// <param name="symbolProductTypes">Symbol product types</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult> SetKillSwitchAsync(string symbol, int timeout, IEnumerable<OrderProductType>? symbolProductTypes = null, CancellationToken ct = default);

        /// <summary>
        /// Get the status of enabled kill switches
        /// <para><a href="https://docs.whitebit.com/private/http-trade-v4/#status-kill-switch-timer" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="ct">Cancellation token</param>
        Task<WebCallResult<IEnumerable<WhiteBitKillSwitch>>> GetKillSwitchStatusAsync(string? symbol = null, CancellationToken ct = default);
    }
}

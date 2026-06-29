using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.RateLimiting.Guards;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc />
    internal class WhiteBitRestClientV4ApiTrading : IWhiteBitRestClientV4ApiTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;
        private readonly ILogger _logger;

        internal WhiteBitRestClientV4ApiTrading(ILogger logger, WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
            _logger = logger;
        }

        #region Place Order

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitOrder>> PlaceSpotOrderAsync(
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
            CancellationToken ct = default)
        {
            if (quoteQuantity != null && type != NewOrderType.Market && side != OrderSide.Buy)
                return HttpResult.Fail<WhiteBitOrder>(WhiteBitExchange.ExchangeName, ArgumentError.Invalid(nameof(quoteQuantity), "quoteQuantity parameter only supported for buy market orders"));

            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("side", side);
            parameters.Add("amount", quantity ?? quoteQuantity ?? 0);
            parameters.Add("price", price);
            parameters.Add("clientOrderId", clientOrderId);
            parameters.Add("postOnly", postOnly);
            parameters.Add("ioc", immediateOrCancel);
            parameters.Add("bboRole", bboRole);
            parameters.Add("activation_price", triggerPrice);
            parameters.Add("stp", stpMode);

            string path;
            if (type == NewOrderType.Limit)
            {
                path = "/api/v4/order/new";
            }
            else if (type == NewOrderType.Market)
            {
                // Buy market orders with base asset, stock endpoint
                // Buy market orders with quote asset, normal endpoint
                // Sell market orders with base asset, normal endpoint
                // Sell market orders with quote asset, normal endpoint
                if (side == OrderSide.Buy && quantity != null)
                    path = "/api/v4/order/stock_market";
                else
                    path = "/api/v4/order/market";
            }
            else if (type == NewOrderType.StopLimit)
            {
                path = "/api/v4/order/stop_limit";
            }
            else if (type == NewOrderType.StopMarket)
            {
                path = "/api/v4/order/stop_market";
            }
            else
            {
                throw new ArgumentException("Unknown path for order type");
            }

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, path, WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Place Multiple Order

        /// <inheritdoc />
        public async Task<HttpResult<CallResult<WhiteBitOrderResponse>[]>> PlaceSpotMultipleOrdersAsync(
            IEnumerable<WhiteBitOrderRequest> requests,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("orders", requests.ToArray());
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/bulk", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var resultData = await _baseClient.SendAsync<WhiteBitOrderResponse[]>(request, parameters, ct).ConfigureAwait(false);
            if (!resultData.Success)
                return HttpResult.Fail<CallResult<WhiteBitOrderResponse>[]>(resultData);

            var result = new List<CallResult<WhiteBitOrderResponse>>();
            foreach (var item in resultData.Data)
            {
                if (item.Error?.Code == null)
                {
                    result.Add(CallResult.Ok(item));
                }
                else
                {
                    ServerError error;
                    if (item.Error.Errors?.Any() == true)
                        error = new ServerError(item.Error.Code, _baseClient.GetErrorInfo(item.Error.Code, string.Join(", ", item.Error.Errors!.Select(x => $"Error field '{x.Key}': {string.Join(" & ", x.Value)}"))));
                    else
                        error = new ServerError(item.Error.Code, _baseClient.GetErrorInfo(item.Error.Code, item.Error.Message!));

                    result.Add(CallResult.Fail<WhiteBitOrderResponse>(error));
                }
            }

            if (result.All(x => !x.Success))
                return HttpResult.Fail(resultData, new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return HttpResult.Ok(resultData, result.ToArray());
        }

        #endregion

        #region Cancel Order

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitOrder>> CancelOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("orderId", orderId);
            parameters.Add("clientOrderId", clientOrderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/cancel", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel Orders

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitCancelOrdersResult[]>> CancelOrdersAsync(IEnumerable<WhiteBitCancelRequest> requests, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("orders", requests.ToArray());
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/cancel/bulk", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCancelOrdersResult[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel All Orders

        /// <inheritdoc />
        public async Task<HttpResult> CancelAllOrdersAsync(string? symbol = null, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.AddRaw("type", orderProductTypes?.Select(EnumConverter.GetString).ToArray());
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/cancel/all", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Open Orders

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitOrder[]>> GetOpenOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("orderId", orderId);
            parameters.Add("clientOrderId", clientOrderId);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/orders", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Closed Orders

        /// <inheritdoc />
        public async Task<HttpResult<Dictionary<string, WhiteBitClosedOrder[]>>> GetClosedOrdersAsync(
            string? symbol = null, 
            long? orderId = null, 
            string? clientOrderId = null,
            ClosedOrderStatus? status = null,
            int? limit = null, 
            int? offset = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("orderId", orderId);
            parameters.Add("clientOrderId", clientOrderId);
            parameters.Add("status", status);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            parameters.Add("startDate", startTime);
            parameters.Add("endDate", endTime);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/trade-account/order/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitClosedOrder[]>>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return result;

            if (result.Data == null)
                return HttpResult.Ok(result, new Dictionary<string, WhiteBitClosedOrder[]>());

            foreach (var symbolOrders in result.Data)
            {
                foreach (var order in symbolOrders.Value)
                    order.Symbol = symbolOrders.Key;
            }

            return result;
        }

        #endregion

        #region Get User Trades

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitUserTrade[]>> GetUserTradesAsync(string? symbol = null, string? clientOrderId = null, DateTime? startDate = null, DateTime? endDate = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("clientOrderId", clientOrderId);
            parameters.Add("startTime", startDate);
            parameters.Add("endTime", endDate);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/trade-account/executed-history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            if (symbol != null)
            {
                var result = await _baseClient.SendAsync<WhiteBitUserTrade[]>(request, parameters, ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await _baseClient.SendAsync<Dictionary<string, WhiteBitUserTrade[]>>(request, parameters, ct).ConfigureAwait(false);
                if (!result.Success)
                    return HttpResult.Fail<WhiteBitUserTrade[]>(result);

                foreach (var item in result.Data)
                {
                    foreach (var x in item.Value)
                        x.Symbol = item.Key;
                }

                return HttpResult.Ok(result, result.Data.Values.SelectMany(x => x).OrderByDescending(x => x.Time).ToArray());
            }
        }

        #endregion

        #region Get Order Trades

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitUserTrade[]>> GetOrderTradesAsync(long orderId, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("orderId", orderId);
            parameters.Add("limit", limit);
            parameters.Add("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/trade-account/order", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOffsetResult<WhiteBitUserTrade>>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<WhiteBitUserTrade[]>(result);

            return HttpResult.Ok(result, result.Data.Records);
        }

        #endregion

        #region Edit Order

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitOrder>> EditOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, decimal? quantity = null, decimal? quoteQuantity = null, decimal? price = null, decimal? triggerPrice = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("orderId", orderId);
            parameters.Add("clientOrderId", clientOrderId);
            parameters.Add("market", symbol);
            parameters.Add("amount", quantity);
            parameters.Add("total", quoteQuantity);
            parameters.Add("price", price);
            parameters.Add("activationPrice", triggerPrice);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/modify", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Set Kill Switch

        /// <inheritdoc />
        public async Task<HttpResult> SetKillSwitchAsync(string symbol, int timeout, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
#pragma warning disable CS8604 // Possible null reference argument. Works as intended
            parameters.Add("timeout", timeout == 0 ? null : timeout.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
            parameters.AddRaw("types", orderProductTypes?.Select(EnumConverter.GetString).ToArray());
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/kill-switch", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Kill Switch

        /// <inheritdoc />
        public async Task<HttpResult<WhiteBitKillSwitch[]>> GetKillSwitchStatusAsync(string? symbol = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(WhiteBitExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v4/order/kill-switch/status", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitKillSwitch[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion
    }
}

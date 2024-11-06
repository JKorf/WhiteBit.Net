using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Enums;
using System;
using WhiteBit.Net.Objects.Models;
using System.Linq;
using CryptoExchange.Net.Converters.SystemTextJson;
using WhiteBit.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.RateLimiting.Guards;

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
        public async Task<WebCallResult<WhiteBitOrder>> PlaceSpotOrderAsync(
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
            CancellationToken ct = default)
        {
            if (quoteQuantity != null && type != NewOrderType.Market && side != OrderSide.Buy)
                return new WebCallResult<WhiteBitOrder>(new ArgumentError("quoteQuantity parameter only supported for buy market orders"));

            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.AddEnum("side", side);
            parameters.AddString("amount", quantity ?? quoteQuantity ?? 0);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("clientOrderId", clientOrderId);
            parameters.AddOptional("postOnly", postOnly);
            parameters.AddOptional("ioc", immediateOrCancel);
            parameters.AddOptional("bboRole", bboRole);
            parameters.AddOptionalString("activationPrice", triggerPrice);

            string path;
            if (type == NewOrderType.Limit)
                path = "/api/v4/order/new";
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
                path = "/api/v4/order/stop_limit";
            else if (type == NewOrderType.StopMarket)
                path = "/api/v4/order/stop_market";
            else
                throw new ArgumentException("Unknown path for order type");

            var request = _definitions.GetOrCreate(HttpMethod.Post, path, WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion


        #region Place Multiple Order

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitOrderResponse>>> PlaceSpotMultipleOrdersAsync(
            IEnumerable<WhiteBitOrderRequest> requests,
            CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("orders", requests);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/bulk", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitOrderResponse>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel Order

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitOrder>> CancelOrderAsync(string symbol, long id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.Add("orderId", id);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/cancel", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel All Orders

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllOrdersAsync(string? symbol = null, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("type", orderProductTypes?.Select(EnumConverter.GetString));
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/cancel/all", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Open Orders

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitOrder>>> GetOpenOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("orderId", orderId);
            parameters.AddOptional("clientOrderId", clientOrderId);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/orders", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitOrder>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Closed Orders

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, IEnumerable<WhiteBitClosedOrder>>>> GetClosedOrdersAsync(string? symbol = null, long? orderId = null, string? clientOrderId = null, ClosedOrderStatus? status = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("orderId", orderId);
            parameters.AddOptional("clientOrderId", clientOrderId);
            parameters.AddOptionalEnum("status", status);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/trade-account/order/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<Dictionary<string, IEnumerable<WhiteBitClosedOrder>>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result;

            foreach(var symbolOrders in result.Data)
            {
                foreach (var order in symbolOrders.Value)
                    order.Symbol = symbolOrders.Key;
            }
            
            return result;
        }

        #endregion

        #region Get User Trades

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitUserTrade>>> GetUserTradesAsync(string? symbol = null, string? clientOrderId = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("clientOrderId", clientOrderId);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/trade-account/executed-history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            if (symbol != null)
            {
                var result = await _baseClient.SendAsync<IEnumerable<WhiteBitUserTrade>>(request, parameters, ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await _baseClient.SendAsync<Dictionary<string, IEnumerable<WhiteBitUserTrade>>>(request, parameters, ct).ConfigureAwait(false);
                if (!result)
                    return result.As<IEnumerable<WhiteBitUserTrade>>(default);

                foreach(var item in result.Data)
                {
                    foreach (var x in item.Value)
                        x.Symbol = item.Key;
                }

                return result.As<IEnumerable<WhiteBitUserTrade>>(result.Data.Values.SelectMany(x => x).OrderByDescending(x => x.Time).ToArray());
            }
        }

        #endregion

        #region Get Order Trades

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitUserTrade>>> GetOrderTradesAsync(long orderId, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("orderId", orderId);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/trade-account/order", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOffsetResult<WhiteBitUserTrade>>(request, parameters, ct).ConfigureAwait(false);
            return result.As<IEnumerable<WhiteBitUserTrade>>(result.Data?.Records);
        }

        #endregion

        #region Edit Order

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitOrder>> EditOrderAsync(string symbol, long orderId, decimal? quantity = null, decimal? quoteQuantity = null, decimal? price = null, decimal? triggerPrice = null, string? clientOrderId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("orderId", orderId);
            parameters.Add("market", symbol);
            parameters.AddOptionalString("amount", quantity);
            parameters.AddOptionalString("total", quoteQuantity);
            parameters.AddOptionalString("price", price);
            parameters.AddOptionalString("activationPrice", triggerPrice);
            parameters.AddOptional("clientOrderId", clientOrderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/modify", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Set Kill Switch

        /// <inheritdoc />
        public async Task<WebCallResult> SetKillSwitchAsync(string symbol, int timeout, IEnumerable<OrderProductType>? orderProductTypes = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
#pragma warning disable CS8604 // Possible null reference argument. Works as intended
            parameters.Add("timeout", timeout == 0 ? null : timeout.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
            parameters.AddOptional("types", orderProductTypes?.Select(EnumConverter.GetString));
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/kill-switch", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Kill Switch

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitKillSwitch>>> GetKillSwitchStatusAsync(string? symbol = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/kill-switch/status", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitKillSwitch>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion
    }
}

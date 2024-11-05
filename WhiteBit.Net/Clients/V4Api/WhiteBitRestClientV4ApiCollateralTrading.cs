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
    internal class WhiteBitRestClientV4ApiCollateralTrading : IWhiteBitRestClientV4ApiCollateralTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;
        private readonly ILogger _logger;

        internal WhiteBitRestClientV4ApiCollateralTrading(ILogger logger, WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
            _logger = logger;
        }


        #region Place Order

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitCollateralOrder>> PlaceOrderAsync(
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
            CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.AddEnum("side", side);
            parameters.AddOptionalString("amount", quantity);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("clientOrderId", clientOrderId);
            parameters.AddOptional("postOnly", postOnly);
            parameters.AddOptional("ioc", immediateOrCancel);
            parameters.AddOptional("bboRole", bboRole);
            parameters.AddOptionalString("activationPrice", triggerPrice);
            parameters.AddOptionalString("stopLoss", stopLossPrice);
            parameters.AddOptionalString("takeProfit", takeProfitPrice);

            string path;
            if (type == NewOrderType.Limit)
                path = "/api/v4/order/collateral/limit";
            else if (type == NewOrderType.Market)
                path = "/api/v4/order/collateral/market";            
            else if (type == NewOrderType.StopLimit)
                path = "/api/v4/order/collateral/stop-limit";
            else if (type == NewOrderType.StopMarket)
                path = "/api/v4/order/collateral/trigger-market";
            else
                throw new ArgumentException("Unknown path for order type");

            var request = _definitions.GetOrCreate(HttpMethod.Post, path, WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitCollateralOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Open Positions

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitPosition>>> GetOpenPositionsAsync(string? symbol = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/positions/open", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitPosition>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Position History

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<WhiteBitPositionHistory>>> GetPositionHistoryAsync(string? symbol = null, long? positionId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("positionId", positionId);
            parameters.AddOptionalMilliseconds("startDate", startTime);
            parameters.AddOptionalMilliseconds("endDate", endTime);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/collateral-account/positions/history", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(12000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<IEnumerable<WhiteBitPositionHistory>>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Open Conditional Orders

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitConditionalOrdersResult>> GetOpenConditionalOrdersAsync(string symbol, int? limit = null, int? offset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("offset", offset);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/conditional-orders", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(1000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitConditionalOrdersResult>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Place Oco Order

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitOcoOrder>> PlaceOcoOrderAsync(string symbol, OrderSide orderSide, decimal quantity, decimal price, decimal triggerPrice, decimal stopLimitPrice, string? clientOrderId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.AddEnum("side", orderSide);
            parameters.AddString("amount", quantity);
            parameters.AddString("price", price);
            parameters.AddString("activation_price", triggerPrice);
            parameters.AddString("stop_limit_price", stopLimitPrice);
            parameters.AddOptional("clientOrderId", clientOrderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/collateral/oco", WhiteBitExchange.RateLimiter.WhiteBit, 1, true);
            var result = await _baseClient.SendAsync<WhiteBitOcoOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel Oco Order

        /// <inheritdoc />
        public async Task<WebCallResult<WhiteBitOcoOrder>> CancelOcoOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.Add("orderId", orderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/oco-cancel", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<WhiteBitOcoOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel Conditional Order

        /// <inheritdoc />
        public async Task<WebCallResult> CancelConditionalOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.Add("id", orderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/conditional-cancel", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Cancel OTO Order

        /// <inheritdoc />
        public async Task<WebCallResult> CancelOTOOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.Add("market", symbol);
            parameters.Add("otoId", orderId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "/api/v4/order/oto-cancel", WhiteBitExchange.RateLimiter.WhiteBit, 1, true,
                limitGuard: new SingleLimitGuard(10000, TimeSpan.FromSeconds(10), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}

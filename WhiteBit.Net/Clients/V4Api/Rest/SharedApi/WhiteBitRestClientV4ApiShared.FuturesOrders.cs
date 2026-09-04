using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhiteBit.Net.Enums;
using WhiteBit.Net.ExtensionMethods;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitRestClientV4SharedApi
    {

        public SharedFeeDeductionType FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        public SharedFeeAssetType FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        public SharedOrderType[] FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.LimitMaker, SharedOrderType.Market };
        public SharedTimeInForce[] FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        public SharedQuantitySupport FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);
        #region Place Futures Order

        async Task<ICallResult<SharedId>> IPlaceFuturesOrder.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
            => await PlaceFuturesOrderAsync(request, ct).ConfigureAwait(false);

        public PlaceFuturesOrderOptions PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                (request.OrderType == SharedOrderType.Limit || request.OrderType == SharedOrderType.LimitMaker) ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                takeProfitPrice: request.TakeProfitPrice,
                stopLossPrice: request.StopLossPrice,
                positionSide: request.PositionSide.HasValue ? request.PositionSide.Value.ToPositionSide() : null,
                reduceOnly: request.ReduceOnly,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        #endregion
        #region Get Futures Order

        async Task<ICallResult<SharedFuturesOrder>> IGetFuturesOrder.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
            => await GetFuturesOrderAsync(request, ct).ConfigureAwait(false);

        public GetFuturesOrderOptions GetFuturesOrderOptions { get; } = new GetFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedFuturesOrder>> GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await _api.Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, openOrder.Symbol), 
                    openOrder.Symbol,
                    openOrder.OrderId.ToString(),
                    ParseOrderType(openOrder.OrderType, openOrder.PostOnly),
                    openOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    SharedOrderStatus.Open,
                    openOrder.CreateTime)
                {
                    ClientOrderId = openOrder.ClientOrderId == string.Empty ? null : openOrder.ClientOrderId,
                    AveragePrice = openOrder.QuantityFilled != 0 ? openOrder.QuoteQuantityFilled / openOrder.QuantityFilled : null,
                    OrderPrice = openOrder.Price == 0 ? null : openOrder.Price,
                    OrderQuantity = new SharedOrderQuantity(openOrder.Quantity, contractQuantity: openOrder.Quantity),
                    QuantityFilled = new SharedOrderQuantity(openOrder.QuantityFilled, openOrder.QuoteQuantityFilled, openOrder.QuantityFilled),
                    TimeInForce = ParseTimeInForce(openOrder),
#pragma warning disable CS0618 // Type or member is obsolete
                    Fee = openOrder.Fee,
                    FeeAsset = openOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TakeProfitPrice = openOrder.OtoData?.TakeProfit,
                    StopLossPrice = openOrder.OtoData?.StopLoss,
                    TriggerPrice = openOrder.TriggerPrice,
                    IsTriggerOrder = openOrder.TriggerPrice > 0,
                    UpdateTime = openOrder.UpdateTime
                });
            }
            else
            {
                var closeOrders = await _api.Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closeOrders);

                if (!closeOrders.Data.Any())
                    return HttpResult.Fail<SharedFuturesOrder>(closeOrders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status is OrderStatus.Canceled or OrderStatus.AutoCanceledUserMargin 
                    ? SharedOrderStatus.Canceled 
                    : SharedOrderStatus.Filled;

                return HttpResult.Ok(closeOrders, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, closedOrder.Symbol), 
                    closedOrder.Symbol,
                    closedOrder.OrderId.ToString(),
                    ParseOrderType(closedOrder.OrderType, closedOrder.PostOnly),
                    closedOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    status,
                    closedOrder.CreateTime)
                    {
                        ClientOrderId = closedOrder.ClientOrderId == string.Empty ? null : closedOrder.ClientOrderId,
                        AveragePrice = closedOrder.QuantityFilled != 0 ? closedOrder.QuoteQuantityFilled / closedOrder.QuantityFilled : null,
                        OrderPrice = closedOrder.Price == 0 ? null : closedOrder.Price,
                        OrderQuantity = new SharedOrderQuantity(closedOrder.Quantity, contractQuantity: closedOrder.Quantity),
                        QuantityFilled = new SharedOrderQuantity(closedOrder.QuantityFilled, closedOrder.QuoteQuantityFilled, closedOrder.QuantityFilled),
                        TimeInForce = ParseTimeInForce(closedOrder),
#pragma warning disable CS0618 // Type or member is obsolete
                        Fee = closedOrder.Fee,
                        FeeAsset = closedOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TakeProfitPrice = closedOrder.OtoData?.TakeProfit,
                        StopLossPrice = closedOrder.OtoData?.StopLoss,
                        TriggerPrice = closedOrder.TriggerPrice,
                        IsTriggerOrder = closedOrder.TriggerPrice > 0,
                        UpdateTime = closedOrder.FillTime ?? closedOrder.UpdateTime
                });
            }            
        }

        #endregion
        #region Get Open Futures Orders

        async Task<ICallResult<SharedFuturesOrder[]>> IGetOpenFuturesOrders.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
            => await GetOpenFuturesOrdersAsync(request, ct).ConfigureAwait(false);

        public GetOpenFuturesOrdersOptions GetOpenFuturesOrdersOptions { get; } = new GetOpenFuturesOrdersOptions(_exchange, true);
        public async Task<HttpResult<SharedFuturesOrder[]>> GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var allOpenOrders = new List<WhiteBitOrder>();
            int offset = 0;
            HttpResult<WhiteBitOrder[]> orders;
            while (true)
            {
                orders = await _api.Trading.GetOpenOrdersAsync(symbol, limit: 100, offset: offset, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                    return HttpResult.Fail<SharedFuturesOrder[]>(orders);

                allOpenOrders.AddRange(orders.Data);
                if (orders.Data.Length == 100)
                    offset += 100;
                else
                    break;
            }

            var data = allOpenOrders.Where(x => x.Symbol.EndsWith("_PERP"));

            return HttpResult.Ok<SharedFuturesOrder[]>(orders, [.. data.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                SharedOrderStatus.Open,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled, x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x),
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0,
                UpdateTime = x.UpdateTime
            })]);
        }

        #endregion
        #region Get Closed Futures Orders

        async Task<ICallResult<SharedFuturesOrder[]>> IGetClosedFuturesOrders.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetClosedFuturesOrdersAsync(request, pageRequest, ct).ConfigureAwait(false);

        public GetFuturesClosedOrdersOptions GetClosedFuturesOrdersOptions { get; } = new GetFuturesClosedOrdersOptions(_exchange, false, true, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedFuturesOrder[]>> GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, maxPeriod: TimeSpan.FromDays(31));

            // Get data
            var result = await _api.Trading.GetClosedOrdersAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
               () => Pagination.NextPageFromOffset(pageParams, limit),
               result.Data.Values.Count,
               result.Data.Values.SelectMany(x => x.Select(x => x.CreateTime)),
               request.StartTime,
               request.EndTime ?? DateTime.UtcNow,
               pageParams,
               TimeSpan.FromDays(31),
               TimeSpan.FromDays(180));

            var data = result.Data.Where(x => x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, xk.Key), 
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status is OrderStatus.Canceled or OrderStatus.AutoCanceledUserMargin
                    ? SharedOrderStatus.Canceled
                    : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled, x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x),
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0,
                UpdateTime = x.FillTime ?? x.UpdateTime
            }));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.CreateTime!.Value, request.StartTime, request.EndTime, direction).ToArray(), nextPageRequest);
        }

        #endregion
        #region Get Futures Order Trades

        async Task<ICallResult<SharedUserTrade[]>> IGetFuturesOrderTrades.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
            => await GetFuturesOrderTradesAsync(request, ct).ConfigureAwait(false);

        public GetFuturesOrderTradesOptions GetFuturesOrderTradesOptions { get; } = new GetFuturesOrderTradesOptions(_exchange, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                request.Symbol!.GetSymbol(FormatSymbol),
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.OrderSide == null ? (SharedOrderSide?)null : x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.Time)
            {
                ClientOrderId = x.ClientOrderId,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        #endregion

        #region Get Futures User Trade History

        async Task<ICallResult<SharedUserTrade[]>> IGetFuturesUserTradeHistory.GetFuturesUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetFuturesUserTradeHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetFuturesUserTradeHistoryAsync(request, pageRequest, ct);
        GetFuturesUserTradeHistoryOptions IFuturesOrderRestClient.GetFuturesUserTradesOptions => GetFuturesUserTradeHistoryOptions;

        public GetFuturesUserTradeHistoryOptions GetFuturesUserTradeHistoryOptions { get; } = new GetFuturesUserTradeHistoryOptions(_exchange, false, true, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetFuturesUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Length,
                result.Data.Select(x => x.Time),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams,
                maxAge: TimeSpan.FromDays(180));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Time, request.StartTime, request.EndTime, direction)
                       .Select(y => 
                           new SharedUserTrade(
                                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, y.Symbol), 
                                y.Symbol,
                                y.OrderId.ToString(),
                                y.Id.ToString(),
                                y.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                                new SharedOrderQuantity(y.Quantity),
                                y.Price,
                                y.Time)
                            {
                                ClientOrderId = y.ClientOrderId,
                                Fee = y.Fee,
                                FeeAsset = y.FeeAsset,
                                Role = y.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
                            })
                       .ToArray(), nextPageRequest);
        }

        #endregion
        #region Cancel Futures Order

        async Task<ICallResult<SharedId>> ICancelFuturesOrder.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
            => await CancelFuturesOrderAsync(request, ct).ConfigureAwait(false);

        public CancelFuturesOrderOptions CancelFuturesOrderOptions { get; } = new CancelFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.OrderId.ToString()));
        }

        #endregion
        #region Get Positions

        async Task<ICallResult<SharedPosition[]>> IGetPositions.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
            => await GetPositionsAsync(request, ct).ConfigureAwait(false);

        public GetPositionsOptions GetPositionsOptions { get; } = new GetPositionsOptions(_exchange, true);
        public async Task<HttpResult<SharedPosition[]>> GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = GetPositionsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPosition[]>(Exchange, validationError);

            var result = await _api.CollateralTrading.GetOpenPositionsAsync(symbol: request.Symbol?.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPosition[]>(result);

            var data = result.Data;
            var resultTypes = request.Symbol == null && request.TradingMode == null ? SupportedTradingModes : request.Symbol != null ? new[] { request.Symbol.TradingMode } : new[] { request.TradingMode!.Value };
            return HttpResult.Ok(result, data.Select(x =>
            new SharedPosition(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                x.Symbol,
                new SharedOrderQuantity(Math.Abs(x.Quantity)),
                x.UpdateTime)
            {
                UnrealizedPnl = x.Pnl,
                LiquidationPrice = x.LiquidationPrice == 0 ? null : x.LiquidationPrice,
                AverageOpenPrice = x.BasePrice,
                PositionMode = SharedPositionMode.OneWay,
                PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short, 
                TakeProfitPrice = x.TpSl?.TakeProfitPrice,
                StopLossPrice = x.TpSl?.StopLossPrice
            }).ToArray());
        }

        #endregion
        #region Close Position

        async Task<ICallResult<SharedId>> IClosePosition.ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
            => await ClosePositionAsync(request, ct).ConfigureAwait(false);

        public ClosePositionOptions ClosePositionOptions { get; } = new ClosePositionOptions(_exchange, true)
        {
            RequiredRequestParameters = [
                RequestParameter<ClosePositionRequest>.Required(x => x.PositionSide, "Current side of the position to close", SharedPositionSide.Long),
                RequestParameter<ClosePositionRequest>.Required(x => x.Quantity,"Quantity of the position is required", 0.1m)
                ]
        };
        public async Task<HttpResult<SharedId>> ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = ClosePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.CollateralTrading.PlaceOrderAsync(
                symbol,
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                NewOrderType.Market,
                request.Quantity,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        #endregion
    }
}

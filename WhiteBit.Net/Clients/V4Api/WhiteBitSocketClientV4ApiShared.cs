using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using CryptoExchange.Net.Objects.Sockets;
using System.Linq;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4Api : IWhiteBitSocketClientV4ApiShared
    {
        private const string _topicSpotId = "WhiteBitSpot";
        private const string _topicFuturesId = "WhiteBitFutures";
        public string Exchange => "WhiteBit";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public TradingMode[] SupportedFuturesModes => new[] { TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Balance client
        EndpointOptions<SubscribeBalancesRequest> IBalanceSocketClient.SubscribeBalanceOptions { get; } = new EndpointOptions<SubscribeBalancesRequest>(false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("BalanceAssets", typeof(List<string>), "The assets to subscribe for updates", new List<string>{ "USDT", "ETH", "BTC" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<ExchangeEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = ((IBalanceSocketClient)this).SubscribeBalanceOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
            if (assets == null)
            {
                // request all assets
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var assetsResult = await client.V4Api.ExchangeData.GetAssetsAsync().ConfigureAwait(false);
                if (!assetsResult)
                    return new ExchangeResult<UpdateSubscription>(Exchange, assetsResult.Error!);

                assets = assetsResult.Data.Where(x => x.CanDeposit).Select(x => x.Asset).ToList();
            }

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var result = await SubscribeToSpotBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.AsExchangeEvent<SharedBalance[]>(Exchange, update.Data.Select(x => new SharedBalance(x.Key, x.Value.Available, x.Value.Available + x.Value.Frozen)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return new ExchangeResult<UpdateSubscription>(Exchange, result);
            }
            else
            {
                var result = await SubscribeToMarginBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.AsExchangeEvent<SharedBalance[]>(Exchange, update.Data.Select(x => new SharedBalance(x.Asset, x.AvailableWithoutBorrow, x.Balance)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return new ExchangeResult<UpdateSubscription>(Exchange, result);
            }
        }

        #endregion

        #region Book Ticker client

        EndpointOptions<SubscribeBookTickerRequest> IBookTickerSocketClient.SubscribeBookTickerOptions { get; } = new EndpointOptions<SubscribeBookTickerRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<ExchangeEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = ((IBookTickerSocketClient)this).SubscribeBookTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToBookTickerUpdatesAsync(symbol, update =>
            {
                handler(update.AsExchangeEvent(Exchange, new SharedBookTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, update.Data.Symbol),
                    update.Data.Symbol,
                    update.Data.BestAskPrice,
                    update.Data.BestAskQuantity,
                    update.Data.BestBidPrice,
                    update.Data.BestBidQuantity)));
            }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(false,
            SharedKlineInterval.OneMinute,
            SharedKlineInterval.ThreeMinutes,
            SharedKlineInterval.FiveMinutes,
            SharedKlineInterval.FifteenMinutes,
            SharedKlineInterval.ThirtyMinutes,
            SharedKlineInterval.OneHour,
            SharedKlineInterval.TwoHours,
            SharedKlineInterval.FourHours,
            SharedKlineInterval.SixHours,
            SharedKlineInterval.TwelveHours,
            SharedKlineInterval.OneDay,
            SharedKlineInterval.OneWeek,
            SharedKlineInterval.OneMonth);
        async Task<ExchangeResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<ExchangeEvent<SharedKline>> handler, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeResult<UpdateSubscription>(Exchange, ArgumentError.Invalid(nameof(GetKlinesRequest.Interval), "Interval not supported"));

            var validationError = ((IKlineSocketClient)this).SubscribeKlineOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToKlineUpdatesAsync(symbol, interval, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                foreach(var item in update.Data)
                    handler(update.AsExchangeEvent(Exchange, new SharedKline(request.Symbol, symbol, item.OpenTime, item.ClosePrice, item.HighPrice, item.LowPrice, item.OpenPrice, item.Volume)));
            }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Ticker client
        SubscribeTickerOptions ITickerSocketClient.SubscribeTickerOptions { get; } = new SubscribeTickerOptions()
        {
            SupportsMultipleSymbols = true
        };
        async Task<ExchangeResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<ExchangeEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = ((ITickerSocketClient)this).SubscribeTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = request.Symbols?.Length > 0 ? request.Symbols.Select(x => x.GetSymbol(FormatSymbol)).ToArray() : [request.Symbol!.GetSymbol(FormatSymbol)];
            var result = await SubscribeToTickerUpdatesAsync(symbols, update => handler(update.AsExchangeEvent(Exchange,
                new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicSpotId, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, update.Data.Symbol), update.Data.Symbol, update.Data.Ticker.LastPrice, update.Data.Ticker.HighPrice, update.Data.Ticker.LowPrice, update.Data.Ticker.Volume, update.Data.Ticker.OpenPrice == 0 ? null : Math.Round(update.Data.Ticker.ClosePrice / update.Data.Ticker.OpenPrice * 100 - 100, 2))
                {
                    QuoteVolume = update.Data.Ticker.QuoteVolume
                })), ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Trade client

        EndpointOptions<SubscribeTradeRequest> ITradeSocketClient.SubscribeTradeOptions { get; } = new EndpointOptions<SubscribeTradeRequest>(false)
        {
            SupportsMultipleSymbols = true,
            MaxSymbolCount = 100
        };
        async Task<ExchangeResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<ExchangeEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = ((ITradeSocketClient)this).SubscribeTradeOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = request.Symbols?.Length > 0 ? request.Symbols.Select(x => x.GetSymbol(FormatSymbol)).ToArray() : [request.Symbol!.GetSymbol(FormatSymbol)];
            var result = await SubscribeToTradeUpdatesAsync(symbols, 
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.AsExchangeEvent<SharedTrade[]>(Exchange, update.Data.Trades.Select(x => 
                    new SharedTrade(ExchangeSymbolCache.ParseSymbol(_topicSpotId, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, update.Data.Symbol), update.Data.Symbol, x.Quantity, x.Price, x.Timestamp)
                    {
                        Side = x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
                    } ).ToArray()));
                }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region User Trade client

        EndpointOptions<SubscribeUserTradeRequest> IUserTradeSocketClient.SubscribeUserTradeOptions { get; } = new EndpointOptions<SubscribeUserTradeRequest>(true)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("UserTradeSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT", "ETH_PERP" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<ExchangeEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var validationError = ((IUserTradeSocketClient)this).SubscribeUserTradeOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "UserTradeSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult)
                    return new ExchangeResult<UpdateSubscription>(Exchange, symbolsResult.Error!);

                symbols = symbolsResult.Data.Select(x => x.Name).ToList();
            }

            var result = await SubscribeToUserTradeUpdatesAsync(symbols!,
                update =>
                {
                    if (request.TradingMode != null)
                    {
                        if (request.TradingMode == TradingMode.Spot ? update.Data.Symbol.EndsWith("_PERP") : !update.Data.Symbol.EndsWith("_PERP"))
                            return;
                    }

                    handler(update.AsExchangeEvent<SharedUserTrade[]>(Exchange, [
                        new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, update.Data.Symbol),
                            update.Data.Symbol,
                            update.Data.OrderId.ToString(),
                            update.Data.Id.ToString(),
                            update.Data.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            update.Data.Quantity,
                            update.Data.Price,
                            update.Data.Time)
                    {
                        ClientOrderId = update.Data.ClientOrderId,
                        Fee = update.Data.Fee
                    }]));
                }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region Spot Order client

        EndpointOptions<SubscribeSpotOrderRequest> ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; } = new EndpointOptions<SubscribeSpotOrderRequest>(false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<ExchangeEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((ISpotOrderSocketClient)this).SubscribeSpotOrderOptions.ValidateRequest(Exchange, request, TradingMode.Spot, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult)
                    return new ExchangeResult<UpdateSubscription>(Exchange, symbolsResult.Error!);

                symbols = symbolsResult.Data.Where(x => x.SymbolType == SymbolType.Spot).Select(x => x.Name).ToList();
            }

            var result = await SubscribeToOpenOrderUpdatesAsync(symbols!,
                update =>
                {
                    if (update.Data.Order.OrderType != Enums.OrderType.Market
                        && update.Data.Order.OrderType != Enums.OrderType.MarketBase
                        && update.Data.Order.OrderType != Enums.OrderType.StopMarket
                        && update.Data.Order.OrderType != Enums.OrderType.Limit
                        && update.Data.Order.OrderType != Enums.OrderType.StopLimit)
                    {
                        // Futures update
                        return;
                    }

                    handler(update.AsExchangeEvent<SharedSpotOrder[]>(Exchange, new[] {
                        new SharedSpotOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, update.Data.Order.Symbol),
                            update.Data.Order.Symbol,
                            update.Data.Order.OrderId.ToString(),
                            ParseOrderType(update.Data.Order.OrderType, update.Data.Order.PostOnly),
                            update.Data.Order.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(update.Data),
                            update.Data.Order.CreateTime)
                        {
                            ClientOrderId = update.Data.Order.ClientOrderId,
                            OrderPrice = update.Data.Order.Price == 0 ? null : update.Data.Order.Price,
                            OrderQuantity = new SharedOrderQuantity((update.Data.Order.OrderType == OrderType.Market || update.Data.Order.OrderType == OrderType.StopMarket) && update.Data.Order.OrderSide == OrderSide.Buy ? null : update.Data.Order.Quantity, (update.Data.Order.OrderType == OrderType.Market || update.Data.Order.OrderType == OrderType.StopMarket) && update.Data.Order.OrderSide == OrderSide.Buy ? update.Data.Order.Quantity : null),
                            QuantityFilled = new SharedOrderQuantity(update.Data.Order.QuantityFilled, update.Data.Order.QuoteQuantityFilled),                            
                            Fee = update.Data.Order.Fee,
                            FeeAsset = update.Data.Order.FeeAsset,
                            TimeInForce = ParseTimeInForce(update.Data.Order),
                            AveragePrice = update.Data.Order.QuantityFilled == 0 ? null : update.Data.Order.QuoteQuantityFilled / update.Data.Order.QuantityFilled,
                            TriggerPrice = update.Data.Order.TriggerPrice,
                            IsTriggerOrder = update.Data.Order.TriggerPrice > 0,
                            UpdateTime = update.Data.Order.UpdateTime
                        }
                    }));
                },
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Position client
        EndpointOptions<SubscribePositionRequest> IPositionSocketClient.SubscribePositionOptions { get; } = new EndpointOptions<SubscribePositionRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IPositionSocketClient.SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<ExchangeEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = ((IPositionSocketClient)this).SubscribePositionOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToPositionUpdatesAsync(
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.AsExchangeEvent<SharedPosition[]>(Exchange, update.Data.Records.Select(x => new SharedPosition(ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), x.Symbol, Math.Abs(x.Quantity), x.UpdateTime)
                    {
                        AverageOpenPrice = x.BasePrice,
                        PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                        UnrealizedPnl = x.UnrealizedPnl,
                        LiquidationPrice = x.LiquidationPrice,
                    }).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region Futures Order client

        EndpointOptions<SubscribeFuturesOrderRequest> IFuturesOrderSocketClient.SubscribeFuturesOrderOptions { get; } = new EndpointOptions<SubscribeFuturesOrderRequest>(false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_PERP" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<ExchangeEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderSocketClient)this).SubscribeFuturesOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult)
                    return new ExchangeResult<UpdateSubscription>(Exchange, symbolsResult.Error!);

                symbols = symbolsResult.Data.Where(x => x.SymbolType == SymbolType.Futures).Select(x => x.Name).ToList();
            }

            var result = await SubscribeToOpenOrderUpdatesAsync(symbols!,
                update =>
                {
                    if (update.Data.Order.OrderType == Enums.OrderType.Market
                        || update.Data.Order.OrderType == Enums.OrderType.MarketBase
                        || update.Data.Order.OrderType == Enums.OrderType.StopMarket
                        || update.Data.Order.OrderType == Enums.OrderType.Limit
                        || update.Data.Order.OrderType == Enums.OrderType.StopLimit
                        || update.Data.Order.OrderType == OrderType.CollateralNormalization)
                    {
                        // Spot update
                        return;
                    }

                    handler(update.AsExchangeEvent<SharedFuturesOrder[]>(Exchange, new[] {
                        new SharedFuturesOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicFuturesId, update.Data.Order.Symbol),
                            update.Data.Order.Symbol,
                            update.Data.Order.OrderId.ToString(),
                            ParseOrderType(update.Data.Order.OrderType, update.Data.Order.PostOnly),
                            update.Data.Order.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(update.Data),
                            update.Data.Order.CreateTime)
                        {
                            ClientOrderId = update.Data.Order.ClientOrderId,
                            OrderPrice = update.Data.Order.Price == 0 ? null : update.Data.Order.Price,
                            OrderQuantity = new SharedOrderQuantity(update.Data.Order.Quantity, contractQuantity: update.Data.Order.Quantity),
                            QuantityFilled = new SharedOrderQuantity(update.Data.Order.QuantityFilled, update.Data.Order.QuoteQuantityFilled, update.Data.Order.QuantityFilled),
                            Fee = update.Data.Order.Fee,
                            FeeAsset = update.Data.Order.FeeAsset,
                            TimeInForce = ParseTimeInForce(update.Data.Order),
                            AveragePrice = update.Data.Order.QuantityFilled == 0 ? null : update.Data.Order.QuoteQuantityFilled / update.Data.Order.QuantityFilled,
                            TriggerPrice = update.Data.Order.TriggerPrice,
                            IsTriggerOrder = update.Data.Order.TriggerPrice > 0,
                            UpdateTime = update.Data.Order.UpdateTime
                        }
                    }));
                },
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        private SharedOrderStatus ParseOrderStatus(WhiteBitOrderUpdate update)
        {
            if (update.Event == OrderEvent.New)
                return SharedOrderStatus.Open;

            if (update.Order.OrderType == OrderType.Market
                || update.Order.OrderType == OrderType.MarketBase
                || update.Order.OrderType == OrderType.CollateralMarket
                || update.Order.QuantityRemaining == 0)
            {
                return SharedOrderStatus.Filled;
            }

            return SharedOrderStatus.Canceled;
        }

        private SharedOrderType ParseOrderType(OrderType type, bool postOnly)
        {
            if (type == OrderType.Market || type == OrderType.CollateralMarket || type == OrderType.CollateralTriggerStopMarket) return SharedOrderType.Market;
            if (type == OrderType.MarketBase) return SharedOrderType.Market;
            if ((type == OrderType.Limit || type == OrderType.CollateralLimit) && postOnly) return SharedOrderType.LimitMaker;
            if (type == OrderType.Limit || type == OrderType.CollateralLimit || type == OrderType.CollateralStopLimit) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(WhiteBitOrder order)
        {
            if (order.ImmediateOrCancel == true)
                return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }
    }
}

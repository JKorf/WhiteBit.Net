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
using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4Api : IWhiteBitSocketClientV4ApiShared
    {
        private const string _exchange = "WhiteBit";
        private const string _topicSpotId = "WhiteBitSpot";
        private const string _topicFuturesId = "WhiteBitFutures";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public TradingMode[] SupportedFuturesModes => new[] { TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(WhiteBitExchange.Metadata, this);

        #region Balance client
        SubscribeBalanceOptions IBalanceSocketClient.SubscribeBalanceOptions { get; } = new SubscribeBalanceOptions(_exchange, true)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("BalanceAssets", typeof(List<string>), "The assets to subscribe for updates", new List<string>{ "USDT", "ETH", "BTC" })
            }
        };
        async Task<WebSocketResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
                if (assets == null)
                {
                    // request all assets
                    var client = new WhiteBitRestClient(x =>
                    {
                        x.Environment = ClientOptions.Environment;
                    });
                    var assetsResult = await client.V4Api.ExchangeData.GetAssetsAsync().ConfigureAwait(false);
                    if (!assetsResult.Success)
                        return WebSocketResult.Fail<UpdateSubscription>(Exchange, assetsResult.Error!);

                    assets = assetsResult.Data.Where(x => x.CanDeposit).Select(x => x.Asset).ToList();
                }

                var result = await SubscribeToSpotBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.ToType<SharedBalance[]>(update.Data.Select(x =>
                        new SharedBalance(TradingMode.Spot, x.Key, x.Value.Available, x.Value.Available + x.Value.Frozen)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
                if (assets == null)
                {
                    // request all assets
                    var client = new WhiteBitRestClient(x =>
                    {
                        x.Environment = ClientOptions.Environment;
                        x.ApiCredentials = (WhiteBitCredentials?)AuthenticationProvider!.ApiCredentials.Copy();
                    });
                    var assetsResult = await client.V4Api.Account.GetCollateralBalancesAsync().ConfigureAwait(false);
                    if (!assetsResult.Success)
                        return WebSocketResult.Fail<UpdateSubscription>(Exchange, assetsResult.Error!);

                    assets = assetsResult.Data.Select(x => x.Key).Distinct().ToList();
                }

                var result = await SubscribeToMarginBalanceUpdatesAsync(
                    assets,
                    update => handler(update.ToType<SharedBalance[]>(update.Data.Select(x => 
                        new SharedBalance(
                            SupportedFuturesModes, x.Asset, x.AvailableWithoutBorrow, x.Balance)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return result;
            }
        }

        #endregion

        #region Book Ticker client

        SubscribeBookTickerOptions IBookTickerSocketClient.SubscribeBookTickerOptions { get; } = new SubscribeBookTickerOptions(_exchange, false);
        async Task<WebSocketResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToBookTickerUpdatesAsync(symbol, update =>
            {
                handler(update.ToType(new SharedBookTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, update.Data.Symbol),
                    update.Data.Symbol,
                    update.Data.BestAskPrice,
                    update.Data.BestAskQuantity,
                    update.Data.BestBidPrice,
                    update.Data.BestBidQuantity)));
            }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(_exchange, false,
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
        async Task<WebSocketResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<DataEvent<SharedKline>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeKlineOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var interval = (Enums.KlineInterval)request.Interval;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToKlineUpdatesAsync(symbol, interval, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                foreach(var item in update.Data)
                    handler(update.ToType(new SharedKline(request.Symbol, symbol, item.OpenTime, item.ClosePrice, item.HighPrice, item.LowPrice, item.OpenPrice, item.Volume)));
            }, ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Ticker client
        SubscribeTickerOptions ITickerSocketClient.SubscribeTickerOptions { get; } = new SubscribeTickerOptions(_exchange)
        {
            SupportsMultipleSymbols = true
        };
        async Task<WebSocketResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<DataEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = request.Symbols?.Length > 0 ? request.Symbols.Select(x => x.GetSymbol(FormatSymbol)).ToArray() : [request.Symbol!.GetSymbol(FormatSymbol)];
            var result = await SubscribeToTickerUpdatesAsync(symbols, update => handler(update.ToType(
                new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, update.Data.Symbol), update.Data.Symbol, update.Data.Ticker.LastPrice, update.Data.Ticker.HighPrice, update.Data.Ticker.LowPrice, update.Data.Ticker.Volume, update.Data.Ticker.OpenPrice == 0 ? null : Math.Round(update.Data.Ticker.ClosePrice / update.Data.Ticker.OpenPrice * 100 - 100, 2))
                {
                    QuoteVolume = update.Data.Ticker.QuoteVolume
                })), ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Trade client

        SubscribeTradeOptions ITradeSocketClient.SubscribeTradeOptions { get; } = new SubscribeTradeOptions(_exchange, false)
        {
            SupportsMultipleSymbols = true,
            MaxSymbolCount = 100
        };
        async Task<WebSocketResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = request.Symbols?.Length > 0 ? request.Symbols.Select(x => x.GetSymbol(FormatSymbol)).ToArray() : [request.Symbol!.GetSymbol(FormatSymbol)];
            var result = await SubscribeToTradeUpdatesAsync(symbols, 
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.ToType<SharedTrade[]>(update.Data.Trades.Select(x => 
                    new SharedTrade(ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, update.Data.Symbol), update.Data.Symbol, x.Quantity, x.Price, x.Timestamp)
                    {
                        Side = x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
                    } ).ToArray()));
                }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region User Trade client

        SubscribeUserTradeOptions IUserTradeSocketClient.SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchange, true)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("UserTradeSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT", "ETH_PERP" })
            }
        };
        async Task<WebSocketResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeUserTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "UserTradeSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult.Success)
                    return WebSocketResult.Fail<UpdateSubscription>(Exchange, symbolsResult.Error!);

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

                    handler(update.ToType<SharedUserTrade[]>([
                        new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, update.Data.Symbol),
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

            return result;
        }

        #endregion

        #region Spot Order client

        SubscribeSpotOrderOptions ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; } = new SubscribeSpotOrderOptions(_exchange, false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT" })
            }
        };
        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult.Success)
                    return WebSocketResult.Fail<UpdateSubscription>(Exchange, symbolsResult.Error!);

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

                    handler(update.ToType<SharedSpotOrder[]>(new[] {
                        new SharedSpotOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Order.Symbol),
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

            return result;
        }
        #endregion

        #region Position client
        SubscribePositionOptions IPositionSocketClient.SubscribePositionOptions { get; } = new SubscribePositionOptions(_exchange, false);
        async Task<WebSocketResult<UpdateSubscription>> IPositionSocketClient.SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToPositionUpdatesAsync(
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.ToType<SharedPosition[]>(update.Data.Records.Select(x => new SharedPosition(ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol), x.Symbol, Math.Abs(x.Quantity), x.UpdateTime)
                    {
                        AverageOpenPrice = x.BasePrice,
                        PositionMode = SharedPositionMode.OneWay,
                        PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                        UnrealizedPnl = x.UnrealizedPnl,
                        LiquidationPrice = x.LiquidationPrice,
                    }).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Futures Order client

        SubscribeFuturesOrderOptions IFuturesOrderSocketClient.SubscribeFuturesOrderOptions { get; } = new SubscribeFuturesOrderOptions(_exchange, false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_PERP" })
            }
        };
        async Task<WebSocketResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
        {
            var validationError = SharedClient.SubscribeFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult.Success)
                    return WebSocketResult.Fail<UpdateSubscription>(Exchange, symbolsResult.Error!);

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

                    handler(update.ToType<SharedFuturesOrder[]>(new[] {
                        new SharedFuturesOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, update.Data.Order.Symbol),
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

            return result;
        }
        #endregion

        private SharedOrderStatus ParseOrderStatus(WhiteBitOrderUpdate update)
        {
            if (update.Event == OrderEvent.New
                || update.Order.Status == OrderStatus.Open
                || update.Order.Status == OrderStatus.PartiallyFilled)
            {
                return SharedOrderStatus.Open;
            }

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

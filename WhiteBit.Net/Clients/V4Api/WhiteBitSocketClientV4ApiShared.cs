using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using CryptoExchange.Net.Objects.Sockets;
using System.Linq;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4Api : IWhiteBitSocketClientV4ApiShared
    {
        public string Exchange => "WhiteBit";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public TradingMode[] SupportedFuturesModes => new[] { TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();


        #region Balance client
        EndpointOptions<SubscribeBalancesRequest> IBalanceSocketClient.SubscribeBalanceOptions { get; } = new EndpointOptions<SubscribeBalancesRequest>(false)
        {
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("BalanceAssets", typeof(List<string>), "The assets to subscribe for updates", new List<string>{ "USDT", "ETH", "BTC" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<ExchangeEvent<IEnumerable<SharedBalance>>> handler, CancellationToken ct)
        {
            var validationError = ((IBalanceSocketClient)this).SubscribeBalanceOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var result = await SubscribeToSpotBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.AsExchangeEvent<IEnumerable<SharedBalance>>(Exchange, update.Data.Select(x => new SharedBalance(x.Key, x.Value.Available, x.Value.Available + x.Value.Frozen)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return new ExchangeResult<UpdateSubscription>(Exchange, result);
            }
            else
            {
                var result = await SubscribeToMarginBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.AsExchangeEvent<IEnumerable<SharedBalance>>(Exchange, update.Data.Select(x => new SharedBalance(x.Asset, x.AvailableWithoutBorrow, x.Balance)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return new ExchangeResult<UpdateSubscription>(Exchange, result);
            }
        }

        #endregion

        #region Book Ticker client

        EndpointOptions<SubscribeBookTickerRequest> IBookTickerSocketClient.SubscribeBookTickerOptions { get; } = new EndpointOptions<SubscribeBookTickerRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<ExchangeEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = ((IBookTickerSocketClient)this).SubscribeBookTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            decimal lastBidPrice = 0;
            decimal lastBidQuantity = 0;
            decimal lastAskPrice = 0;
            decimal lastAskQuantity = 0;
            var result = await SubscribeToOrderBookUpdatesAsync(symbol, 1, update =>
            {
                var ask = update.Data.OrderBook.Asks.SingleOrDefault(x => x.Quantity != 0);
                var bid = update.Data.OrderBook.Bids.SingleOrDefault(x => x.Quantity != 0);
                handler(update.AsExchangeEvent(Exchange, new SharedBookTicker(
                    ask?.Price ?? lastAskPrice, ask?.Quantity ?? lastAskQuantity,
                    bid?.Price ?? lastBidPrice, bid?.Quantity ?? lastBidQuantity)));
                lastBidPrice = bid?.Price ?? lastBidPrice;
                lastBidQuantity = bid?.Quantity ?? lastBidQuantity;
                lastAskPrice = ask?.Price ?? lastAskPrice;
                lastAskQuantity = ask?.Quantity ?? lastAskQuantity;
            }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(false);
        async Task<ExchangeResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<ExchangeEvent<SharedKline>> handler, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeResult<UpdateSubscription>(Exchange, new ArgumentError("Interval not supported"));

            var validationError = ((IKlineSocketClient)this).SubscribeKlineOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var result = await SubscribeToKlineUpdatesAsync(symbol, interval, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                foreach(var item in update.Data)
                    handler(update.AsExchangeEvent(Exchange, new SharedKline(item.OpenTime, item.ClosePrice, item.HighPrice, item.LowPrice, item.OpenPrice, item.Volume)));
            }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Ticker client
        EndpointOptions<SubscribeTickerRequest> ITickerSocketClient.SubscribeTickerOptions { get; } = new EndpointOptions<SubscribeTickerRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<ExchangeEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = ((ITickerSocketClient)this).SubscribeTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(update.AsExchangeEvent(Exchange, new SharedSpotTicker(update.Data.Symbol, update.Data.Ticker.LastPrice, update.Data.Ticker.HighPrice, update.Data.Ticker.LowPrice, update.Data.Ticker.Volume, update.Data.Ticker.OpenPrice == 0 ? null : Math.Round(update.Data.Ticker.ClosePrice / update.Data.Ticker.OpenPrice * 100 - 100, 2)))), ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Trade client

        EndpointOptions<SubscribeTradeRequest> ITradeSocketClient.SubscribeTradeOptions { get; } = new EndpointOptions<SubscribeTradeRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<ExchangeEvent<IEnumerable<SharedTrade>>> handler, CancellationToken ct)
        {
            var validationError = ((ITradeSocketClient)this).SubscribeTradeOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var result = await SubscribeToTradeUpdatesAsync(symbol, 
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.AsExchangeEvent<IEnumerable<SharedTrade>>(Exchange, update.Data.Trades.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)
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
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("UserTradeSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT", "ETH_PERP" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<ExchangeEvent<IEnumerable<SharedUserTrade>>> handler, CancellationToken ct)
        {
            var validationError = ((IUserTradeSocketClient)this).SubscribeUserTradeOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "UserTradeSymbols");
            var result = await SubscribeToUserTradeUpdatesAsync(symbols!,
                update =>
                {
                    if (request.TradingMode != null)
                    {
                        if (request.TradingMode == TradingMode.Spot ? update.Data.Symbol.EndsWith("_PERP") : !update.Data.Symbol.EndsWith("_PERP"))
                            return;
                    }

                    handler(update.AsExchangeEvent<IEnumerable<SharedUserTrade>>(Exchange, [new SharedUserTrade(update.Data.Symbol, update.Data.OrderId.ToString(), update.Data.Id.ToString(), update.Data.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell, update.Data.Quantity, update.Data.Price, update.Data.Time)
                    {
                        Fee = update.Data.Fee
                    }]));
                }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }

        #endregion

        #region Spot Order client

        EndpointOptions<SubscribeSpotOrderRequest> ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; } = new EndpointOptions<SubscribeSpotOrderRequest>(false)
        {
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<ExchangeEvent<IEnumerable<SharedSpotOrder>>> handler, CancellationToken ct)
        {
            var validationError = ((ISpotOrderSocketClient)this).SubscribeSpotOrderOptions.ValidateRequest(Exchange, request, TradingMode.Spot, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            var result = await SubscribeToOpenOrderUpdatesAsync(assets!,
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

                    handler(update.AsExchangeEvent<IEnumerable<SharedSpotOrder>>(Exchange, new[] {
                        new SharedSpotOrder(
                            update.Data.Order.Symbol,
                            update.Data.Order.OrderId.ToString(),
                            ParseOrderType(update.Data.Order.OrderType, update.Data.Order.PostOnly),
                            update.Data.Order.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(update.Data),
                            update.Data.Order.CreateTime)
                        {
                            ClientOrderId = update.Data.Order.ClientOrderId,
                            OrderPrice = update.Data.Order.Price,
                            Quantity = update.Data.Order.OrderType == OrderType.Market && update.Data.Order.OrderSide == OrderSide.Buy ? null : update.Data.Order.Quantity,
                            QuantityFilled = update.Data.Order.QuantityFilled,
                            QuoteQuantity = update.Data.Order.OrderType == OrderType.Market && update.Data.Order.OrderSide == OrderSide.Buy ? update.Data.Order.Quantity : null,
                            QuoteQuantityFilled = update.Data.Order.QuoteQuantityFilled,
                            Fee = update.Data.Order.Fee,
                            TimeInForce = ParseTimeInForce(update.Data.Order),
                            AveragePrice = update.Data.Order.QuantityFilled == 0 ? null : update.Data.Order.QuoteQuantityFilled / update.Data.Order.QuantityFilled
                        }
                    }));
                },
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Position client
        EndpointOptions<SubscribePositionRequest> IPositionSocketClient.SubscribePositionOptions { get; } = new EndpointOptions<SubscribePositionRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IPositionSocketClient.SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<ExchangeEvent<IEnumerable<SharedPosition>>> handler, CancellationToken ct)
        {
            var validationError = ((IPositionSocketClient)this).SubscribePositionOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToPositionUpdatesAsync(
                update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.AsExchangeEvent<IEnumerable<SharedPosition>>(Exchange, update.Data.Records.Select(x => new SharedPosition(x.Symbol, Math.Abs(x.Quantity), x.updateTime)
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
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_PERP" })
            }
        };
        async Task<ExchangeResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<ExchangeEvent<IEnumerable<SharedFuturesOrder>>> handler, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderSocketClient)this).SubscribeFuturesOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            var result = await SubscribeToOpenOrderUpdatesAsync(assets!,
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

                    handler(update.AsExchangeEvent<IEnumerable<SharedFuturesOrder>>(Exchange, new[] {
                        new SharedFuturesOrder(
                            update.Data.Order.Symbol,
                            update.Data.Order.OrderId.ToString(),
                            ParseOrderType(update.Data.Order.OrderType, update.Data.Order.PostOnly),
                            update.Data.Order.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(update.Data),
                            update.Data.Order.CreateTime)
                        {
                            ClientOrderId = update.Data.Order.ClientOrderId,
                            OrderPrice = update.Data.Order.Price,
                            Quantity = update.Data.Order.OrderType == OrderType.Market && update.Data.Order.OrderSide == OrderSide.Buy ? null : update.Data.Order.Quantity,
                            QuantityFilled = update.Data.Order.QuantityFilled,
                            QuoteQuantity = update.Data.Order.OrderType == OrderType.Market && update.Data.Order.OrderSide == OrderSide.Buy ? update.Data.Order.Quantity : null,
                            QuoteQuantityFilled = update.Data.Order.QuoteQuantityFilled,
                            Fee = update.Data.Order.Fee,
                            TimeInForce = ParseTimeInForce(update.Data.Order),
                            AveragePrice = update.Data.Order.QuantityFilled == 0 ? null : update.Data.Order.QuoteQuantityFilled / update.Data.Order.QuantityFilled
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
                return SharedOrderStatus.Filled;

            return SharedOrderStatus.Canceled;
        }

        private SharedOrderType ParseOrderType(OrderType type, bool postOnly)
        {
            if (type == OrderType.Market || type == OrderType.CollateralMarket) return SharedOrderType.Market;
            if (type == OrderType.MarketBase) return SharedOrderType.Market;
            if ((type == OrderType.Limit || type == OrderType.CollateralLimit) && postOnly) return SharedOrderType.LimitMaker;
            if (type == OrderType.Limit || type == OrderType.CollateralLimit) return SharedOrderType.Limit;

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

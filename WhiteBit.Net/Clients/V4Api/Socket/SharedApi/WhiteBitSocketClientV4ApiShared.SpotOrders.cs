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
    internal partial class WhiteBitSocketClientV4SharedApi
    {
        #region Spot Order client

        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
            => await SubscribeToSpotOrderUpdatesAsync(request, x => handler(x.ToType<SharedSpotOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeSpotOrderOptions SubscribeSpotOrderOptions { get; } = new SubscribeSpotOrderOptions(_exchange, false)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("OrderSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT" })
            }
        };
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "OrderSymbols");
            if (symbols == null)
            {
                // request all symbols
                var client = new WhiteBitRestClient(x =>
                {
                    x.Environment = _api.ClientOptions.Environment;
                });
                var symbolsResult = await client.V4Api.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolsResult.Success)
                    return WebSocketResult.Fail<UpdateSubscription>(Exchange, symbolsResult.Error!);

                symbols = symbolsResult.Data.Where(x => x.SymbolType == SymbolType.Spot).Select(x => x.Name).ToList();
            }

            var result = await _api.SubscribeToOpenOrderUpdatesAsync(symbols!,
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

                    handler(update.ToType<SharedSpotOrderUpdate[]>(new[] {
                        new SharedSpotOrderUpdate(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, update.Data.Order.Symbol),
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
#pragma warning disable CS0618 // Type or member is obsolete
                            Fee = update.Data.Order.Fee,
                            FeeAsset = update.Data.Order.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
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
    }
}

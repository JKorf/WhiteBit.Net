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
        #region Subscribe User Trades

        public SubscribeUserTradeOptions SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchange, true)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("UserTradeSymbols", typeof(List<string>), "The symbols to subscribe for updates", new List<string>{ "ETH_USDT", "ETH_PERP" })
            }
        };
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeUserTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbols = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "UserTradeSymbols");
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

                symbols = symbolsResult.Data.Select(x => x.Name).ToList();
            }

            var result = await _api.SubscribeToUserTradeUpdatesAsync(symbols!,
                update =>
                {
                    if (request.TradingMode != null)
                    {
                        if (request.TradingMode == TradingMode.Spot ? update.Data.Symbol.EndsWith("_PERP") : !update.Data.Symbol.EndsWith("_PERP"))
                            return;
                    }

                    handler(update.ToType<SharedUserTrade[]>([
                        new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, update.Data.Symbol),
                            update.Data.Symbol,
                            update.Data.OrderId.ToString(),
                            update.Data.Id.ToString(),
                            update.Data.OrderSide == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            new SharedOrderQuantity(update.Data.Quantity),
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
    }
}

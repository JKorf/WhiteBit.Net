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
        #region Subscribe Book Ticker

        public SubscribeBookTickerOptions SubscribeBookTickerOptions { get; } = new SubscribeBookTickerOptions(_exchange, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = SubscribeBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.SubscribeToBookTickerUpdatesAsync(symbol, update =>
            {
                handler(update.ToType(new SharedBookTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, update.Data.Symbol) ?? ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, update.Data.Symbol),
                    update.Data.Symbol,
                    update.Data.BestAskPrice,
                    new SharedOrderQuantity(update.Data.BestAskQuantity),
                    update.Data.BestBidPrice,
                    new SharedOrderQuantity(update.Data.BestBidQuantity))));
            }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}

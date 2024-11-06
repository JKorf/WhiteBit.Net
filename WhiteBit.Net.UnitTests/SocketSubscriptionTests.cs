using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class SocketSubscriptionTests
    {
        [Test]
        public async Task ValidateSpotExchangeDataSubscriptions()
        {
            var client = new WhiteBitSocketClient(opts =>
            {
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new SocketSubscriptionValidator<WhiteBitSocketClient>(client, "Subscriptions/V4", "wss://api.whitebit.com", stjCompare: true);
            await tester.ValidateAsync<WhiteBitTradeUpdate>((client, handler) => client.V4Api.SubscribeToTradeUpdatesAsync("ETH_USDT", handler), "Trades", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitLastPriceUpdate>((client, handler) => client.V4Api.SubscribeToLastPriceUpdatesAsync("ETH_USDT", handler), "LastPrice", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitTickerUpdate>((client, handler) => client.V4Api.SubscribeToTickerUpdatesAsync("ETH_USDT", handler), "Ticker", nestedJsonProperty: "params");
            await tester.ValidateAsync<IEnumerable<WhiteBitKlineUpdate>>((client, handler) => client.V4Api.SubscribeToKlineUpdatesAsync("ETH_USDT", Enums.KlineInterval.OneDay, handler), "Klines", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitBookUpdate>((client, handler) => client.V4Api.SubscribeToOrderBookUpdatesAsync("ETH_USDT", 5, handler), "OrderBook", nestedJsonProperty: "params");
        }
    }
}

using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class SocketSubscriptionTests
    {
        [Test]
        public async Task ValidateV4ApiSubscriptions()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new TraceLoggerProvider());
            var client = new WhiteBitSocketClient(Options.Create(new WhiteBitSocketOptions
            {
                Environment = new WhiteBitEnvironment("UnitTest", "https://localhost", "wss://localhost"),
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456")
            }), loggerFactory);
            var tester = new SocketSubscriptionValidator<WhiteBitSocketClient>(client, "Subscriptions/V4", "wss://api.whitebit.com");
            await tester.ValidateAsync<WhiteBitTradeUpdate>((client, handler) => client.V4Api.SubscribeToTradeUpdatesAsync("ETH_USDT", handler), "Trades", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitLastPriceUpdate>((client, handler) => client.V4Api.SubscribeToLastPriceUpdatesAsync("ETH_USDT", handler), "LastPrice", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitTickerUpdate>((client, handler) => client.V4Api.SubscribeToTickerUpdatesAsync("ETH_USDT", handler), "Ticker", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitKlineUpdate[]>((client, handler) => client.V4Api.SubscribeToKlineUpdatesAsync("ETH_USDT", Enums.KlineInterval.OneDay, handler), "Klines", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitBookUpdate>((client, handler) => client.V4Api.SubscribeToOrderBookUpdatesAsync("ETH_USDT", 5, handler), "OrderBook", nestedJsonProperty: "params");

            await tester.ValidateAsync<Dictionary<string, WhiteBitTradeBalance>>((client, handler) => client.V4Api.SubscribeToSpotBalanceUpdatesAsync(["ETH"], handler), "SpotBalances", nestedJsonProperty: "params", useFirstUpdateItem: true);
            await tester.ValidateAsync<WhiteBitMarginBalance[]>((client, handler) => client.V4Api.SubscribeToMarginBalanceUpdatesAsync(["ETH"], handler), "MarginBalances", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitPositionsUpdate>((client, handler) => client.V4Api.SubscribeToPositionUpdatesAsync(handler), "Positions", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitOrderUpdate>((client, handler) => client.V4Api.SubscribeToOpenOrderUpdatesAsync(["ETH_USDT"], handler, null), "OpenOrders", nestedJsonProperty: "params");
            await tester.ValidateAsync<WhiteBitUserTradeUpdate>((client, handler) => client.V4Api.SubscribeToUserTradeUpdatesAsync(["ETH_USDT"], handler), "UserTrades", nestedJsonProperty: "params");
        }
    }
}

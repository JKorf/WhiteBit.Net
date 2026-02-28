using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class SocketRequestTests
    {
        private WhiteBitSocketClient CreateClient()
        {
            var fact = new LoggerFactory();
            fact.AddProvider(new TraceLoggerProvider());
            var client = new WhiteBitSocketClient(Options.Create(new WhiteBitSocketOptions
            {
                OutputOriginalData = true,
                RequestTimeout = TimeSpan.FromSeconds(5),
                ApiCredentials = new ApiCredentials("123", "123", "123"),
                Environment = new WhiteBitEnvironment("UnitTest", "https://localhost", "wss://localhost")
            }), fact);
            return client;
        }

        [Test]
        public async Task ValidateExchangeApiCalls()
        {
            var tester = new SocketRequestValidator<WhiteBitSocketClient>("Socket/V4Api");

            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetTradeHistoryAsync("ETH-USDT", 1), "GetTradeHistory", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetLastPriceAsync("ETH-USDT"), "GetLastPrice", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetTickerAsync("ETH-USDT"), "GetTicker", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetKlinesAsync("ETH-USDT", KlineInterval.SixHours, DateTime.UtcNow, DateTime.UtcNow), "GetKlines", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetOrderBookAsync("ETH-USDT", 1), "GetOrderBook", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetSpotBalancesAsync(), "GetSpotBalances", nestedJsonProperty: "result", ignoreProperties: [ ], skipResponseValidation: true);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetMarginBalancesAsync(), "GetMarginBalances", nestedJsonProperty: "result", ignoreProperties: [ ], skipResponseValidation: true);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetOpenOrdersAsync("ETH-USDT"), "GetOpenOrders", nestedJsonProperty: "result", ignoreProperties: [ ]);
            await tester.ValidateAsync(CreateClient(), client => client.V4Api.GetClosedOrdersAsync("ETH-USDT"), "GetClosedOrders", nestedJsonProperty: "result", ignoreProperties: [ ]);
        }
    }
}

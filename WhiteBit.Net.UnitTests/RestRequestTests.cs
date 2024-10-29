using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhiteBit.Net.Clients;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateV4ExchangeDataCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/ExchangeData", "https://whitebit.com", IsAuthenticated, stjCompare: true);
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetSystemStatusAsync(), "GetSystemStatus");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetTickersAsync(), "GetTickers", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetOrderBookAsync("ETH_USDT"), "GetOrderBook");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetRecentTradesAsync("ETH_USDT"), "GetRecentTrades");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetCollateralSymbolsAsync(), "GetCollateralSymbols");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetFuturesSymbolsAsync(), "GetFuturesSymbols", nestedJsonProperty: "result");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestUrl?.Contains("signature") == true || result.RequestBody?.Contains("signature=") == true;
        }
    }
}

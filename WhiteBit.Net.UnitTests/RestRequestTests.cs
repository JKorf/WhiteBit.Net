using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteBit.Net.Clients;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateV4AccountCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/Account", "https://whitebit.com", IsAuthenticated, stjCompare: true);
            await tester.ValidateAsync(client => client.V4Api.Account.GetConvertEstimateAsync("123", "123", 0.1m, "123"), "GetConvertEstimate");
            await tester.ValidateAsync(client => client.V4Api.Account.ConfirmConvertAsync("123"), "ConfirmConvert");
            await tester.ValidateAsync(client => client.V4Api.Account.GetConvertHistoryAsync(), "GetConvertHistory");
        }

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
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetCollateralSymbolsAsync(), "GetCollateralSymbols", nestedJsonProperty: "result");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetFuturesSymbolsAsync(), "GetFuturesSymbols", nestedJsonProperty: "result");
        }

        [Test]
        public async Task ValidateV4TradingCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/Trading", "https://whitebit.com", IsAuthenticated, stjCompare: true);
            await tester.ValidateAsync(client => client.V4Api.Trading.GetUserTradesAsync(), "GetUserTrades");
            await tester.ValidateAsync(client => client.V4Api.Trading.GetOrderTradesAsync(123), "GetOrderTrades", nestedJsonProperty: "records");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestHeaders?.Any(x => x.Key == "X-TXC-SIGNATURE") == true;
        }
    }
}

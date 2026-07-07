using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using WhiteBit.Net.Clients;
using WhiteBit.Net.SymbolOrderBooks;

namespace WhiteBit.Net.UnitTests
{
    [NonParallelizable]
    internal class WhiteBitRestIntegrationTests : RestIntegrationTest<WhiteBitRestClient>
    {
        public override bool Run { get; set; } = false;

        public WhiteBitRestIntegrationTests()
        {
        }

        public override WhiteBitRestClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new WhiteBitRestClient(null, loggerFactory, Options.Create(new Objects.Options.WhiteBitRestOptions
            {
                OutputOriginalData = true,
                ApiCredentials = Authenticated ? new WhiteBitCredentials(key, sec) : null
            }));
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;

            var result = await CreateClient().V4Api.ExchangeData.GetRecentTradesAsync("TSTTST", default);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ResponseStatusCode, Is.EqualTo((HttpStatusCode)422));
        }

        [Test]
        public async Task TestAccount()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(client => client.V4Api.Account.GetMainBalancesAsync(default), true);
            //await RunAndCheckResult(warnings, client => client.V4Api.Account.GetDepositAddressAsync("ETH", "ERC20", default), true);
            await RunAndCheckResult(client => client.V4Api.Account.GetSpotBalancesAsync(default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetDepositWithdrawalHistoryAsync(default, default, default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetDepositWithdrawalSettingsAsync(default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetMiningRewardHistoryAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetCollateralBalancesAsync(default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetCollateralBalanceSummaryAsync(default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetCollateralAccountSummaryAsync(default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Account.GetAccountFundingHistoryAsync("ETH_PERP", default, default, default, default, default), true);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestExchangeData()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(client => client.V4Api.ExchangeData.GetServerTimeAsync(default), false);
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetSymbolsAsync(default), false);
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetSystemStatusAsync(default), false);
            await RunAndCheckResult(client => client.V4Api.ExchangeData.GetTickersAsync(default), false);
            await RunAndCheckResult(client => client.V4Api.ExchangeData.GetAssetsAsync(default), false);
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetOrderBookAsync("ETH_USDT", default, default, default), false);
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetRecentTradesAsync("ETH_USDT", default, default), false);
            await RunAndCheckResult(client => client.V4Api.ExchangeData.GetDepositWithdrawalInfoAsync(default), false);
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetCollateralSymbolsAsync(default), false, "result");
            await RunAndCheckResult(warnings, client => client.V4Api.ExchangeData.GetFuturesSymbolsAsync(default), false, "result");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestTrading()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.V4Api.Trading.GetOpenOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Trading.GetClosedOrdersAsync(default, default, default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Trading.GetUserTradesAsync(default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.Trading.GetKillSwitchStatusAsync(default, default), true);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);

        }

        [Test]
        public async Task TestCollateralTrading()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.V4Api.CollateralTrading.GetOpenPositionsAsync(default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.CollateralTrading.GetPositionHistoryAsync(default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.V4Api.CollateralTrading.GetOpenConditionalOrdersAsync(default, default, default, default), true);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);

        }

        [Test]
        public async Task TestOrderBooks()
        {
            await TestOrderBook(new WhiteBitV4SymbolOrderBook("ETH_USDT"));
        }
    }
}

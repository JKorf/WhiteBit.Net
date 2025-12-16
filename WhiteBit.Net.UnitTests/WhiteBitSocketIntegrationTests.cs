using WhiteBit.Net.Clients;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.UnitTests
{
    internal class WhiteBitSocketIntegrationTests : SocketIntegrationTest<WhiteBitSocketClient>
    {
        public override bool Run { get; set; } = false;

        public WhiteBitSocketIntegrationTests()
        {
        }

        public override WhiteBitSocketClient GetClient(ILoggerFactory loggerFactory, bool useUpdatedDeserialization)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new WhiteBitSocketClient(Options.Create(new WhiteBitSocketOptions
            {
                OutputOriginalData = true,
                ApiCredentials = Authenticated ? new CryptoExchange.Net.Authentication.ApiCredentials(key, sec) : null
            }), loggerFactory);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task TestSubscriptions(bool useUpdatedDeserialization)
        {
            await RunAndCheckUpdate<WhiteBitTicker>(useUpdatedDeserialization, (client, updateHandler) => client.V4Api.SubscribeToSpotBalanceUpdatesAsync(new[] { "ETH" }, default, default), false, true);
            await RunAndCheckUpdate<WhiteBitTickerUpdate>(useUpdatedDeserialization, (client, updateHandler) => client.V4Api.SubscribeToTickerUpdatesAsync("ETH_USDT", updateHandler, default), true, false);
        } 
    }
}

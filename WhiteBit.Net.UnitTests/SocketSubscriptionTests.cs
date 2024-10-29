using CryptoExchange.Net.Testing;
using NUnit.Framework;
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
            var tester = new SocketSubscriptionValidator<WhiteBitSocketClient>(client, "Subscriptions/Spot", "XXX", stjCompare: true);
            //await tester.ValidateAsync<WhiteBitModel>((client, handler) => client.SpotApi.SubscribeToXXXUpdatesAsync(handler), "XXX");
        }
    }
}

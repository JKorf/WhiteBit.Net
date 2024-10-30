using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.JsonNet;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using WhiteBit.Net.Clients;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture()]
    public class WhiteBitRestClientTests
    {
        [Test]
        public void CheckSignatureExample1()
        {
            var authProvider = new WhiteBitAuthenticationProvider(new ApiCredentials("XXX", "XXX"));
            var client = (RestApiClient)new WhiteBitRestClient().V4Api;

            CryptoExchange.Net.Testing.TestHelpers.CheckSignature(
                client,
                authProvider,
                HttpMethod.Post,
                "/api/v3/order",
                (uriParams, bodyParams, headers) =>
                {
                    return headers["X-TXC-SIGNATURE"].ToString();
                },
                "e164832d38f40692420d44786f8dbc98a46af816ed6c30dbc5d6178e71e63f491a4b6dbfea93c887f50c570d4b403dd61fe2432908754f49f904d06c51c84680",
                new Dictionary<string, object>
                {
                    { "market", "ETH_USDT" },
                },
                DateTimeConverter.ParseFromLong(1499827319559),
                true,
                false);
        }

        [Test]
        public void CheckInterfaces()
        {
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingRestInterfaces<WhiteBitRestClient>();
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingSocketInterfaces<WhiteBitSocketClient>();
        }
    }
}

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
                    return bodyParams["signature"].ToString();
                },
                "c8db56825ae71d6d79447849e617115f4a920fa2acdcab2b053c4b2838bd6b71",
                new Dictionary<string, object>
                {
                    { "symbol", "LTCBTC" },
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

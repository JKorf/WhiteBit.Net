using System.Collections.Generic;
using System.Net.Http;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.JsonNet;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Internal;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture()]
    public class WhiteBitRestClientTests
    {
        [Test]
        public void CheckSignatureExample1()
        {
            var authProvider = new WhiteBitAuthenticationProvider(new ApiCredentials("XXX", "XXX"), new WhiteBitNonceProvider());
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

        [Test]
        [TestCase(TradeEnvironmentNames.Live, "https://whitebit.com")]
        [TestCase("", "https://whitebit.com")]
        public void TestConstructorEnvironments(string environmentName, string expected)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "WhiteBit:Environment:Name", environmentName },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddWhiteBit(configuration.GetSection("WhiteBit"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IWhiteBitRestClient>();

            var address = client.V4Api.BaseAddress;

            Assert.That(address, Is.EqualTo(expected));
        }

        [Test]
        public void TestConstructorNullEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "WhiteBit", null },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddWhiteBit(configuration.GetSection("WhiteBit"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IWhiteBitRestClient>();

            var address = client.V4Api.BaseAddress;

            Assert.That(address, Is.EqualTo("https://whitebit.com"));
        }

        [Test]
        public void TestConstructorApiOverwriteEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "WhiteBit:Environment:Name", "test" },
                    { "WhiteBit:Rest:Environment:Name", "live" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddWhiteBit(configuration.GetSection("WhiteBit"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IWhiteBitRestClient>();

            var address = client.V4Api.BaseAddress;

            Assert.That(address, Is.EqualTo("https://whitebit.com"));
        }

        [Test]
        public void TestConstructorConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiCredentials:Key", "123" },
                    { "ApiCredentials:Secret", "456" },
                    { "ApiCredentials:PassPhrase", "222" },
                    { "Socket:ApiCredentials:Key", "456" },
                    { "Socket:ApiCredentials:Secret", "789" },
                    { "Socket:ApiCredentials:PassPhrase", "111" },
                    { "Rest:OutputOriginalData", "true" },
                    { "Socket:OutputOriginalData", "false" },
                    { "Rest:Proxy:Host", "host" },
                    { "Rest:Proxy:Port", "80" },
                    { "Socket:Proxy:Host", "host2" },
                    { "Socket:Proxy:Port", "81" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddWhiteBit(configuration);
            var provider = collection.BuildServiceProvider();

            var restClient = provider.GetRequiredService<IWhiteBitRestClient>();
            var socketClient = provider.GetRequiredService<IWhiteBitSocketClient>();

            Assert.That(((BaseApiClient)restClient.V4Api).OutputOriginalData, Is.True);
            Assert.That(((BaseApiClient)socketClient.V4Api).OutputOriginalData, Is.False);
            Assert.That(((BaseApiClient)restClient.V4Api).AuthenticationProvider.ApiKey, Is.EqualTo("123"));
            Assert.That(((BaseApiClient)socketClient.V4Api).AuthenticationProvider.ApiKey, Is.EqualTo("456"));
            Assert.That(((BaseApiClient)restClient.V4Api).ClientOptions.Proxy.Host, Is.EqualTo("host"));
            Assert.That(((BaseApiClient)restClient.V4Api).ClientOptions.Proxy.Port, Is.EqualTo(80));
            Assert.That(((BaseApiClient)socketClient.V4Api).ClientOptions.Proxy.Host, Is.EqualTo("host2"));
            Assert.That(((BaseApiClient)socketClient.V4Api).ClientOptions.Proxy.Port, Is.EqualTo(81));
        }
    }
}

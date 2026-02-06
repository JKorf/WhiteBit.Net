using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc />
    public class WhiteBitUserClientProvider : IWhiteBitUserClientProvider
    {
        private static ConcurrentDictionary<string, IWhiteBitRestClient> _restClients = new ConcurrentDictionary<string, IWhiteBitRestClient>();
        private static ConcurrentDictionary<string, IWhiteBitSocketClient> _socketClients = new ConcurrentDictionary<string, IWhiteBitSocketClient>();

        private readonly IOptions<WhiteBitRestOptions> _restOptions;
        private readonly IOptions<WhiteBitSocketOptions> _socketOptions;
        private readonly HttpClient _httpClient;
        private readonly ILoggerFactory? _loggerFactory;

        /// <inheritdoc />
        public string ExchangeName => WhiteBitExchange.ExchangeName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="optionsDelegate">Options to use for created clients</param>
        public WhiteBitUserClientProvider(Action<WhiteBitOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate).Rest), Options.Create(ApplyOptionsDelegate(optionsDelegate).Socket))
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserClientProvider(
            HttpClient? httpClient,
            ILoggerFactory? loggerFactory,
            IOptions<WhiteBitRestOptions> restOptions,
            IOptions<WhiteBitSocketOptions> socketOptions)
        {
            _httpClient = httpClient ?? new HttpClient();
            _loggerFactory = loggerFactory;
            _restOptions = restOptions;
            _socketOptions = socketOptions;
        }

        /// <inheritdoc />
        public void InitializeUserClient(string userIdentifier, ApiCredentials credentials, WhiteBitEnvironment? environment = null)
        {
            CreateRestClient(userIdentifier, credentials, environment);
            CreateSocketClient(userIdentifier, credentials, environment);
        }

        /// <inheritdoc />
        public void ClearUserClients(string userIdentifier)
        {
            _restClients.TryRemove(userIdentifier, out _);
            _socketClients.TryRemove(userIdentifier, out _);
        }

        /// <inheritdoc />
        public IWhiteBitRestClient GetRestClient(string userIdentifier, ApiCredentials? credentials = null, WhiteBitEnvironment? environment = null)
        {
            if (!_restClients.TryGetValue(userIdentifier, out var client) || client.Disposed)
                client = CreateRestClient(userIdentifier, credentials, environment);

            return client;
        }

        /// <inheritdoc />
        public IWhiteBitSocketClient GetSocketClient(string userIdentifier, ApiCredentials? credentials = null, WhiteBitEnvironment? environment = null)
        {
            if (!_socketClients.TryGetValue(userIdentifier, out var client) || client.Disposed)
                client = CreateSocketClient(userIdentifier, credentials, environment);

            return client;
        }

        private IWhiteBitRestClient CreateRestClient(string userIdentifier, ApiCredentials? credentials, WhiteBitEnvironment? environment)
        {
            var clientRestOptions = SetRestEnvironment(environment);
            var client = new WhiteBitRestClient(_httpClient, _loggerFactory, clientRestOptions);
            if (credentials != null)
            {
                client.SetApiCredentials(credentials);
                _restClients.TryAdd(userIdentifier, client);
            }
            return client;
        }

        private IWhiteBitSocketClient CreateSocketClient(string userIdentifier, ApiCredentials? credentials, WhiteBitEnvironment? environment)
        {
            var clientSocketOptions = SetSocketEnvironment(environment);
            var client = new WhiteBitSocketClient(clientSocketOptions!, _loggerFactory);
            if (credentials != null)
            {
                client.SetApiCredentials(credentials);
                _socketClients.TryAdd(userIdentifier, client);
            }
            return client;
        }

        private IOptions<WhiteBitRestOptions> SetRestEnvironment(WhiteBitEnvironment? environment)
        {
            if (environment == null)
                return _restOptions;

            var newRestClientOptions = new WhiteBitRestOptions();
            var restOptions = _restOptions.Value.Set(newRestClientOptions);
            newRestClientOptions.Environment = environment;
            return Options.Create(newRestClientOptions);
        }

        private IOptions<WhiteBitSocketOptions> SetSocketEnvironment(WhiteBitEnvironment? environment)
        {
            if (environment == null)
                return _socketOptions;

            var newSocketClientOptions = new WhiteBitSocketOptions();
            var restOptions = _socketOptions.Value.Set(newSocketClientOptions);
            newSocketClientOptions.Environment = environment;
            return Options.Create(newSocketClientOptions);
        }

        private static T ApplyOptionsDelegate<T>(Action<T>? del) where T : new()
        {
            var opts = new T();
            del?.Invoke(opts);
            return opts;
        }
    }
}

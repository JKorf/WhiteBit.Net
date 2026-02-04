using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using CryptoExchange.Net.Trackers.UserData.Interfaces;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Interfaces;
using WhiteBit.Net.Interfaces.Clients;

namespace WhiteBit.Net
{
    /// <inheritdoc />
    public class WhiteBitTrackerFactory : IWhiteBitTrackerFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitTrackerFactory()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public WhiteBitTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public bool CanCreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval) => false;

        /// <inheritdoc />
        public bool CanCreateTradeTracker(SharedSymbol symbol) => true;

        /// <inheritdoc />
        public IKlineTracker CreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval, int? limit = null, TimeSpan? period = null) => throw new NotImplementedException();

        /// <inheritdoc />
        public ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<IWhiteBitRestClient>() ?? new WhiteBitRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<IWhiteBitSocketClient>() ?? new WhiteBitSocketClient();

            return new TradeTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                restClient.V4Api.SharedClient,
                null,
                socketClient.V4Api.SharedClient,
                symbol,
                limit,
                period
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(SpotUserDataTrackerConfig config)
        {
            var restClient = _serviceProvider?.GetRequiredService<IWhiteBitRestClient>() ?? new WhiteBitRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<IWhiteBitSocketClient>() ?? new WhiteBitSocketClient();
            return new WhiteBitUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<WhiteBitUserSpotDataTracker>>() ?? new NullLogger<WhiteBitUserSpotDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(string userIdentifier, SpotUserDataTrackerConfig config, ApiCredentials credentials, WhiteBitEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<IWhiteBitUserClientProvider>() ?? new WhiteBitUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new WhiteBitUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<WhiteBitUserSpotDataTracker>>() ?? new NullLogger<WhiteBitUserSpotDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(FuturesUserDataTrackerConfig config)
        {
            var restClient = _serviceProvider?.GetRequiredService<IWhiteBitRestClient>() ?? new WhiteBitRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<IWhiteBitSocketClient>() ?? new WhiteBitSocketClient();
            return new WhiteBitUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<WhiteBitUserFuturesDataTracker>>() ?? new NullLogger<WhiteBitUserFuturesDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(string userIdentifier, FuturesUserDataTrackerConfig config, ApiCredentials credentials, WhiteBitEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<IWhiteBitUserClientProvider>() ?? new WhiteBitUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new WhiteBitUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<WhiteBitUserFuturesDataTracker>>() ?? new NullLogger<WhiteBitUserFuturesDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }
    }
}

using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using WhiteBit.Net.Interfaces;
using WhiteBit.Net.Interfaces.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WhiteBit.Net.Clients;

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
    }
}

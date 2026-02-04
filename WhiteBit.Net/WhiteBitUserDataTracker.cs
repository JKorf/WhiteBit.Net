using WhiteBit.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.UserData;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.Logging;

namespace WhiteBit.Net
{
    /// <inheritdoc/>
    public class WhiteBitUserSpotDataTracker : UserSpotDataTracker
    {
        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserSpotDataTracker(
            ILogger<WhiteBitUserSpotDataTracker> logger,
            IWhiteBitRestClient restClient,
            IWhiteBitSocketClient socketClient,
            string? userIdentifier,
            SpotUserDataTrackerConfig config) : base(
                logger,
                restClient.V4Api.SharedClient,
                null,
                restClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                restClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                userIdentifier,
                config)
        {
        }
    }

    /// <inheritdoc/>
    public class WhiteBitUserFuturesDataTracker : UserFuturesDataTracker
    {
        /// <inheritdoc/>
        protected override bool WebsocketPositionUpdatesAreFullSnapshots => true;

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserFuturesDataTracker(
            ILogger<WhiteBitUserFuturesDataTracker> logger,
            IWhiteBitRestClient restClient,
            IWhiteBitSocketClient socketClient,
            string? userIdentifier,
            FuturesUserDataTrackerConfig config) : base(logger,
                restClient.V4Api.SharedClient,
                null,
                restClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                restClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                socketClient.V4Api.SharedClient,
                userIdentifier,
                config)
        {
        }
    }
}

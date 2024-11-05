using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// WhiteBit V4 API endpoints
    /// </summary>
    public interface IWhiteBitRestClientV4Api : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        public IWhiteBitRestClientV4ApiAccount Account { get; }

        /// <summary>
        /// Convert endpoints
        /// </summary>
        public IWhiteBitRestClientV4ApiConvert Convert { get; }

        /// <summary>
        /// Code endpoints
        /// </summary>
        public IWhiteBitRestClientV4ApiCodes Codes { get; }

        /// <summary>
        /// Subaccount endpoints
        /// </summary>
        public IWhiteBitRestClientV4ApiSubAccount SubAccount { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        public IWhiteBitRestClientV4ApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        public IWhiteBitRestClientV4ApiTrading Trading { get; }

        /// <summary>
        /// Endpoints related to collateral (Futures and Spot Margin) orders and trades
        /// </summary>
        public IWhiteBitRestClientV4ApiCollateralTrading CollateralTrading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IWhiteBitRestClientV4ApiShared SharedClient { get; }
    }
}

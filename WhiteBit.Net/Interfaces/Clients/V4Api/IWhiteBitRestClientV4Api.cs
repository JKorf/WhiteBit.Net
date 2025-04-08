using CryptoExchange.Net.Interfaces;
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
        /// <see cref="IWhiteBitRestClientV4ApiAccount"/>
        public IWhiteBitRestClientV4ApiAccount Account { get; }

        /// <summary>
        /// Convert endpoints
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiConvert"/>
        public IWhiteBitRestClientV4ApiConvert Convert { get; }

        /// <summary>
        /// Code endpoints
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiCodes"/>
        public IWhiteBitRestClientV4ApiCodes Codes { get; }

        /// <summary>
        /// Subaccount endpoints
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiSubAccount"/>
        public IWhiteBitRestClientV4ApiSubAccount SubAccount { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiExchangeData"/>
        public IWhiteBitRestClientV4ApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiTrading"/>
        public IWhiteBitRestClientV4ApiTrading Trading { get; }

        /// <summary>
        /// Endpoints related to collateral (Futures and Spot Margin) orders and trades
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4ApiCollateralTrading"/>
        public IWhiteBitRestClientV4ApiCollateralTrading CollateralTrading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IWhiteBitRestClientV4ApiShared SharedClient { get; }
    }
}

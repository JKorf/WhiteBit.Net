using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the WhiteBit Rest API. 
    /// </summary>
    public interface IWhiteBitRestClient : IRestClient
    {        
        /// <summary>
        /// V4 API endpoints
        /// </summary>
        public IWhiteBitRestClientV4Api V4Api { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}

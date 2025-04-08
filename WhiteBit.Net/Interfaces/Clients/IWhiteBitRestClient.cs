using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;
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
        /// <see cref="IWhiteBitRestClientV4Api"/>
        public IWhiteBitRestClientV4Api V4Api { get; }

        /// <summary>
        /// Update specific options
        /// </summary>
        /// <param name="options">Options to update. Only specific options are changeable after the client has been created</param>
        void SetOptions(UpdateOptions options);

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}

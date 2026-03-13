using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the WhiteBit Rest API. 
    /// </summary>
    public interface IWhiteBitRestClient : IRestClient<WhiteBitCredentials>
    {
        /// <summary>
        /// V4 API endpoints
        /// </summary>
        /// <see cref="IWhiteBitRestClientV4Api"/>
        public IWhiteBitRestClientV4Api V4Api { get; }
    }
}

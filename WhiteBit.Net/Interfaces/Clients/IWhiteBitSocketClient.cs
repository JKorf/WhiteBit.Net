using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the WhiteBit websocket API
    /// </summary>
    public interface IWhiteBitSocketClient : ISocketClient<WhiteBitCredentials>
    {
        /// <summary>
        /// V4 API endpoints
        /// </summary>
        /// <see cref="IWhiteBitSocketClientV4Api"/>
        public IWhiteBitSocketClientV4Api V4Api { get; }
    }
}

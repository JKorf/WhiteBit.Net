using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc />
    public class WhiteBitSharedApiClient : IWhiteBitSharedApiClient
    {
        /// <inheritdoc />
        public IWhiteBitRestClientV4SharedApi Rest { get; }
        /// <inheritdoc />
        public IWhiteBitSocketClientV4SharedApi Socket { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSharedApiClient(
            IWhiteBitRestClient restClient,
            IWhiteBitSocketClient socketClient)
        {
            Rest = restClient.V4Api.SharedApi;
            Socket = socketClient.V4Api.SharedApi;
        }
    }
}

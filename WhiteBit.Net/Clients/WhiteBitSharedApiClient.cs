using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Options;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc />
    public class WhiteBitSharedApiClient : SharedApiClientBase, IWhiteBitSharedApiClient
    {
        /// <inheritdoc />
        public IWhiteBitRestClientV4SharedApi V4Rest { get; }
        /// <inheritdoc />
        public IWhiteBitSocketClientV4SharedApi V4Socket { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSharedApiClient(
            IWhiteBitRestClient restClient,
            IWhiteBitSocketClient socketClient,
            IOptions<WhiteBitOptions> options)
            : base(options.Value.SharedApi.PreferredTransport,
                    restClient.V4Api.SharedApi,
                    socketClient.V4Api.SharedApi
                  )
        {
            V4Rest = restClient.V4Api.SharedApi;
            V4Socket = socketClient.V4Api.SharedApi;
        }
    }
}

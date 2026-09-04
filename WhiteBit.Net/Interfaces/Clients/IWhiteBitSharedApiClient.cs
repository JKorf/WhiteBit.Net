using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for the shared REST and WebSocket API implementations of WhiteBit
    /// </summary>
    public interface IWhiteBitSharedApiClient
    {
        /// <summary>
        /// REST shared API implementations
        /// </summary>
        IWhiteBitRestClientV4SharedApi Rest { get; }

        /// <summary>
        /// WebSocket shared API implementations
        /// </summary>
        IWhiteBitSocketClientV4SharedApi Socket { get; }
    }
}

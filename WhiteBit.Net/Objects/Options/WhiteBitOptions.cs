using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;

namespace WhiteBit.Net.Objects.Options
{
    /// <summary>
    /// WhiteBit options
    /// </summary>
    public class WhiteBitOptions : LibraryOptions<WhiteBitRestOptions, WhiteBitSocketOptions, WhiteBitCredentials, WhiteBitEnvironment>
    {
        /// <summary>
        /// Options for Shared API usage
        /// </summary>
        public SharedApiOptions SharedApi { get; set; } = new();
    }
}

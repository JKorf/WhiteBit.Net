using CryptoExchange.Net.Objects.Options;

namespace WhiteBit.Net.Objects.Options
{
    /// <summary>
    /// Options for the WhiteBitRestClient
    /// </summary>
    public class WhiteBitRestOptions : RestExchangeOptions<WhiteBitEnvironment>
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        public static WhiteBitRestOptions Default { get; set; } = new WhiteBitRestOptions()
        {
            Environment = WhiteBitEnvironment.Live,
            AutoTimestamp = true
        };

        
         /// <summary>
        /// V4 API options
        /// </summary>
        public RestApiOptions V4Options { get; private set; } = new RestApiOptions();

        /// <summary>
        /// When true, request nonces are allowed to be out of order by the server at least as they are within a 5 second window. This allows concurrent requests to succeed during high traffic.
        /// </summary>
        public bool EnableNonceWindow { get; set; } = false;

        internal WhiteBitRestOptions Copy()
        {
            var options = Copy<WhiteBitRestOptions>();
            options.EnableNonceWindow = EnableNonceWindow;
            options.V4Options = V4Options.Copy<RestApiOptions>();

            return options;
        }
    }
}

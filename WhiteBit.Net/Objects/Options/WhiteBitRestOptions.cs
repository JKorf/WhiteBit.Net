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


        internal WhiteBitRestOptions Copy()
        {
            var options = Copy<WhiteBitRestOptions>();
            
            options.V4Options = V4Options.Copy<RestApiOptions>();

            return options;
        }
    }
}

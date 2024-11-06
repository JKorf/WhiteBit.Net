using CryptoExchange.Net.Objects.Options;

namespace WhiteBit.Net.Objects.Options
{
    /// <summary>
    /// Options for the WhiteBitSocketClient
    /// </summary>
    public class WhiteBitSocketOptions : SocketExchangeOptions<WhiteBitEnvironment>
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        public static WhiteBitSocketOptions Default { get; set; } = new WhiteBitSocketOptions()
        {
            Environment = WhiteBitEnvironment.Live,
            SocketSubscriptionsCombineTarget = 10
        };

        
         /// <summary>
        /// V4 API options
        /// </summary>
        public SocketApiOptions V4Options { get; private set; } = new SocketApiOptions();


        internal WhiteBitSocketOptions Copy()
        {
            var options = Copy<WhiteBitSocketOptions>();
            
            options.V4Options = V4Options.Copy<SocketApiOptions>();

            return options;
        }
    }
}

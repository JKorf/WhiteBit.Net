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
        internal static WhiteBitSocketOptions Default { get; set; } = new WhiteBitSocketOptions()
        {
            Environment = WhiteBitEnvironment.Live,
            SocketSubscriptionsCombineTarget = 10
        };

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitSocketOptions()
        {
            Default?.Set(this);
        }

        /// <summary>
        /// V4 API options
        /// </summary>
        public SocketApiOptions V4Options { get; private set; } = new SocketApiOptions();


        internal WhiteBitSocketOptions Set(WhiteBitSocketOptions targetOptions)
        {
            targetOptions = base.Set<WhiteBitSocketOptions>(targetOptions);
            targetOptions.V4Options = V4Options.Set(targetOptions.V4Options);
            return targetOptions;
        }
    }
}

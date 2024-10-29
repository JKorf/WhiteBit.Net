using CryptoExchange.Net.Objects;
using WhiteBit.Net.Objects;

namespace WhiteBit.Net
{
    /// <summary>
    /// WhiteBit environments
    /// </summary>
    public class WhiteBitEnvironment : TradeEnvironment
    {
        /// <summary>
        /// Rest API address
        /// </summary>
        public string RestClientAddress { get; }

        /// <summary>
        /// Socket API address
        /// </summary>
        public string SocketClientAddress { get; }

        internal WhiteBitEnvironment(
            string name,
            string restAddress,
            string streamAddress) :
            base(name)
        {
            RestClientAddress = restAddress;
            SocketClientAddress = streamAddress;
        }

        /// <summary>
        /// Live environment
        /// </summary>
        public static WhiteBitEnvironment Live { get; }
            = new WhiteBitEnvironment(TradeEnvironmentNames.Live,
                                     WhiteBitApiAddresses.Default.RestClientAddress,
                                     WhiteBitApiAddresses.Default.SocketClientAddress);

        /// <summary>
        /// Create a custom environment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spotRestAddress"></param>
        /// <param name="spotSocketStreamsAddress"></param>
        /// <returns></returns>
        public static WhiteBitEnvironment CreateCustom(
                        string name,
                        string spotRestAddress,
                        string spotSocketStreamsAddress)
            => new WhiteBitEnvironment(name, spotRestAddress, spotSocketStreamsAddress);
    }
}

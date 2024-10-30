namespace WhiteBit.Net.Objects
{
    /// <summary>
    /// Api addresses
    /// </summary>
    public class WhiteBitApiAddresses
    {
        /// <summary>
        /// The address used by the WhiteBitRestClient for the API
        /// </summary>
        public string RestClientAddress { get; set; } = "";
        /// <summary>
        /// The address used by the WhiteBitSocketClient for the websocket API
        /// </summary>
        public string SocketClientAddress { get; set; } = "";

        /// <summary>
        /// The default addresses to connect to the WhiteBit API
        /// </summary>
        public static WhiteBitApiAddresses Default = new WhiteBitApiAddresses
        {
            RestClientAddress = "https://whitebit.com",
            SocketClientAddress = "wss://api.whitebit.com"
        };
    }
}

using WhiteBit.Net.Clients;
using WhiteBit.Net.Interfaces.Clients;

namespace CryptoExchange.Net.Interfaces
{
    /// <summary>
    /// Extensions for the ICryptoRestClient and ICryptoSocketClient interfaces
    /// </summary>
    public static class CryptoClientExtensions
    {
        /// <summary>
        /// Get the WhiteBit REST Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static IWhiteBitRestClient WhiteBit(this ICryptoRestClient baseClient) => baseClient.TryGet<IWhiteBitRestClient>(() => new WhiteBitRestClient());

        /// <summary>
        /// Get the WhiteBit Websocket Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static IWhiteBitSocketClient WhiteBit(this ICryptoSocketClient baseClient) => baseClient.TryGet<IWhiteBitSocketClient>(() => new WhiteBitSocketClient());
    }
}

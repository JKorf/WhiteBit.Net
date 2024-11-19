using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Objects.Options
{
    /// <summary>
    /// WhiteBit options
    /// </summary>
    public class WhiteBitOptions
    {
        /// <summary>
        /// Rest client options
        /// </summary>
        public WhiteBitRestOptions Rest { get; set; } = new WhiteBitRestOptions();

        /// <summary>
        /// Socket client options
        /// </summary>
        public WhiteBitSocketOptions Socket { get; set; } = new WhiteBitSocketOptions();

        /// <summary>
        /// Trade environment. Contains info about URL's to use to connect to the API. Use `WhiteBitEnvironment` to swap environment, for example `Environment = WhiteBitEnvironment.Live`
        /// </summary>
        public WhiteBitEnvironment? Environment { get; set; }

        /// <summary>
        /// The api credentials used for signing requests.
        /// </summary>
        public ApiCredentials? ApiCredentials { get; set; }

        /// <summary>
        /// The DI service lifetime for the IWhiteBitSocketClient
        /// </summary>
        public ServiceLifetime? SocketClientLifeTime { get; set; }
    }
}

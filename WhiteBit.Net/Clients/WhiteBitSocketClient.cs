using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Logging;
using System;
using WhiteBit.Net.Clients.V4Api;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc cref="IWhiteBitSocketClient" />
    public class WhiteBitSocketClient : BaseSocketClient, IWhiteBitSocketClient
    {
        #region fields
        #endregion

        #region Api clients
                
         /// <inheritdoc />
        public IWhiteBitSocketClientV4Api V4Api { get; }

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of WhiteBitSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        public WhiteBitSocketClient(ILoggerFactory? loggerFactory = null) : this((x) => { }, loggerFactory)
        {
        }

        /// <summary>
        /// Create a new instance of WhiteBitSocketClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public WhiteBitSocketClient(Action<WhiteBitSocketOptions> optionsDelegate) : this(optionsDelegate, null)
        {
        }

        /// <summary>
        /// Create a new instance of WhiteBitSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public WhiteBitSocketClient(Action<WhiteBitSocketOptions>? optionsDelegate, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "WhiteBit")
        {
            var options = WhiteBitSocketOptions.Default.Copy();
            optionsDelegate?.Invoke(options);
            Initialize(options);
                        
            V4Api = AddApiClient(new WhiteBitSocketClientV4Api(_logger, options));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<WhiteBitSocketOptions> optionsDelegate)
        {
            var options = WhiteBitSocketOptions.Default.Copy();
            optionsDelegate(options);
            WhiteBitSocketOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            V4Api.SetApiCredentials(credentials);
        }
    }
}

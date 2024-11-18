using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public WhiteBitSocketClient(Action<WhiteBitSocketOptions>? optionsDelegate = null)
            : this(Options.Create(ApplyOptionsDelegate(optionsDelegate)), null)
        {
        }

        /// <summary>
        /// Create a new instance of WhiteBitSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="options">Option configuration</param>
        public WhiteBitSocketClient(IOptions<WhiteBitSocketOptions> options, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "WhiteBit")
        {
            Initialize(options.Value);

            V4Api = AddApiClient(new WhiteBitSocketClientV4Api(_logger, options.Value));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<WhiteBitSocketOptions> optionsDelegate)
        {
            WhiteBitSocketOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            V4Api.SetApiCredentials(credentials);
        }
    }
}

using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using CryptoExchange.Net.Authentication;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Clients.V4Api;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc cref="IWhiteBitRestClient" />
    public class WhiteBitRestClient : BaseRestClient, IWhiteBitRestClient
    {
        #region Api clients

        
         /// <inheritdoc />
        public IWhiteBitRestClientV4Api V4Api { get; }


        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of the WhiteBitRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public WhiteBitRestClient(Action<WhiteBitRestOptions>? optionsDelegate = null) : this(null, null, optionsDelegate)
        {
        }

        /// <summary>
        /// Create a new instance of the WhiteBitRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public WhiteBitRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, Action<WhiteBitRestOptions>? optionsDelegate = null) : base(loggerFactory, "WhiteBit")
        {
            var options = WhiteBitRestOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            V4Api = AddApiClient(new WhiteBitRestClientV4Api(_logger, httpClient, options));
        }

        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<WhiteBitRestOptions> optionsDelegate)
        {
            var options = WhiteBitRestOptions.Default.Copy();
            optionsDelegate(options);
            WhiteBitRestOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            V4Api.SetApiCredentials(credentials);
        }
    }
}

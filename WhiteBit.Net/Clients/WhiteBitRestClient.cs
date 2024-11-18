using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using CryptoExchange.Net.Authentication;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Clients.V4Api;
using Microsoft.Extensions.Options;

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
        public WhiteBitRestClient(Action<WhiteBitRestOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate)))
        {
        }

        /// <summary>
        /// Create a new instance of the WhiteBitRestClient using provided options
        /// </summary>
        /// <param name="options">Option configuration</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public WhiteBitRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, IOptions<WhiteBitRestOptions> options) : base(loggerFactory, "WhiteBit")
        {
            Initialize(options.Value);

            V4Api = AddApiClient(new WhiteBitRestClientV4Api(_logger, httpClient, options.Value));
        }

        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<WhiteBitRestOptions> optionsDelegate)
        {
            WhiteBitRestOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            V4Api.SetApiCredentials(credentials);
        }
    }
}

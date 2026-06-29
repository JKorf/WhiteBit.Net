using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using CryptoExchange.Net.Clients;

namespace WhiteBit.Net.Clients
{
    /// <inheritdoc />
    public class WhiteBitUserClientProvider : UserClientProvider<
        IWhiteBitRestClient,
        IWhiteBitSocketClient,
        WhiteBitRestOptions,
        WhiteBitSocketOptions,
        WhiteBitCredentials,
        WhiteBitEnvironment
        >, IWhiteBitUserClientProvider
    {
        /// <inheritdoc />
        public override string ExchangeName => WhiteBitExchange.ExchangeName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="optionsDelegate">Options to use for created clients</param>
        public WhiteBitUserClientProvider(Action<WhiteBitOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate).Rest), Options.Create(ApplyOptionsDelegate(optionsDelegate).Socket))
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WhiteBitUserClientProvider(
            HttpClient? httpClient,
            ILoggerFactory? loggerFactory,
            IOptions<WhiteBitRestOptions> restOptions,
            IOptions<WhiteBitSocketOptions> socketOptions)
            : base(httpClient, loggerFactory, restOptions, socketOptions)
        {
        }

        /// <inheritdoc />
        protected override IWhiteBitRestClient ConstructRestClient(HttpClient client, ILoggerFactory? loggerFactory, IOptions<WhiteBitRestOptions> options) 
            => new WhiteBitRestClient(client, loggerFactory, options);
        /// <inheritdoc />
        protected override IWhiteBitSocketClient ConstructSocketClient(ILoggerFactory? loggerFactory, IOptions<WhiteBitSocketOptions> options)
            => new WhiteBitSocketClient(options, loggerFactory);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Clients.MessageHandlers;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc cref="IWhiteBitRestClientV4Api" />
    internal partial class WhiteBitRestClientV4Api : RestApiClient<WhiteBitEnvironment, WhiteBitAuthenticationProvider, WhiteBitCredentials>, IWhiteBitRestClientV4Api
    {
        #region fields 
        /// <inheritdoc />
        public new WhiteBitRestOptions ClientOptions => (WhiteBitRestOptions)base.ClientOptions;

        protected override ErrorMapping ErrorMapping { get; } = WhiteBitErrors.RestErrors;
        protected override IRestMessageHandler MessageHandler { get; } = new WhiteBitRestMessageHandler(WhiteBitErrors.RestErrors);
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiAccount Account { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiConvert Convert { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiCodes Codes { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiSubAccount SubAccount { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiTrading Trading { get; }
        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiCollateralTrading CollateralTrading { get; }
        /// <inheritdoc />
        public string ExchangeName => "WhiteBit";
        #endregion

        #region constructor/destructor
        internal WhiteBitRestClientV4Api(ILogger logger, HttpClient? httpClient, WhiteBitRestOptions options)
            : base(logger, WhiteBitExchange.ExchangeName, httpClient, options.Environment.RestClientAddress, options, options.V4Options)
        {
            Account = new WhiteBitRestClientV4ApiAccount(this);
            Convert = new WhiteBitRestClientV4ApiConvert(this);
            Codes = new WhiteBitRestClientV4ApiCodes(this);
            SubAccount = new WhiteBitRestClientV4ApiSubAccount(this);
            ExchangeData = new WhiteBitRestClientV4ApiExchangeData(logger, this);
            Trading = new WhiteBitRestClientV4ApiTrading(logger, this);
            CollateralTrading = new WhiteBitRestClientV4ApiCollateralTrading(logger, this);
        }
        #endregion

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(WhiteBitExchange._serializerContext);

        /// <inheritdoc />
        protected override WhiteBitAuthenticationProvider CreateAuthenticationProvider(WhiteBitCredentials credentials)
            => new WhiteBitAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new WhiteBitNonceProvider());

        internal async Task<HttpResult> SendAsync(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync<Unit>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result.Success && result.Error is DeserializeError)
                return HttpResult.Ok(result); // Deserialize error without data expected is not an issue

            return result;
        }

        internal async Task<HttpResult<T>> SendAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<T>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        protected override Task<HttpResult<DateTime>> GetServerTimestampAsync()
            => ExchangeData.GetServerTimeAsync();

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverDate = null)
            => WhiteBitExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverDate);

        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiShared SharedClient => this;

    }
}

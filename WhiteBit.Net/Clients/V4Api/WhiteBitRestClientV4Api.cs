using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc cref="IWhiteBitRestClientV4Api" />
    internal partial class WhiteBitRestClientV4Api : RestApiClient, IWhiteBitRestClientV4Api
    {
        #region fields 
        /// <inheritdoc />
        public new WhiteBitRestOptions ClientOptions => (WhiteBitRestOptions)base.ClientOptions;
        internal static TimeSyncState _timeSyncState = new TimeSyncState("V4 Api");
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
            : base(logger, httpClient, options.Environment.RestClientAddress, options, options.V4Options)
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

        private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            PropertyNameCaseInsensitive = false,
            Converters =
                    {
                        new DateTimeConverter(),
                        new EnumConverter(),
                        new BoolConverter(),
                        new DecimalConverter(),
                        new IntConverter(),
                        new LongConverter(),
                        new EmptyArrayObjectConverter<Dictionary<string, IEnumerable<WhiteBitClosedOrder>>>()
                    }
        };
        /// <inheritdoc />
        protected override IStreamMessageAccessor CreateAccessor() => new SystemTextJsonStreamMessageAccessor(_serializerOptions);
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new WhiteBitAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new WhiteBitNonceProvider());

        internal Task<WebCallResult> SendAsync(RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null)
            => SendToAddressAsync(BaseAddress, definition, parameters, cancellationToken, weight);

        internal async Task<WebCallResult> SendToAddressAsync(string baseAddress, RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync(baseAddress, definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        internal Task<WebCallResult<T>> SendAsync<T>(RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
            => SendToAddressAsync<T>(BaseAddress, definition, parameters, cancellationToken, weight);

        internal async Task<WebCallResult<T>> SendToAddressAsync<T>(string baseAddress, RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<T>(baseAddress, definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        internal Task<WebCallResult<T>> SendAsync<T>(RequestDefinition definition, ParameterCollection? uriParameters, ParameterCollection? bodyParameters, CancellationToken cancellationToken, int? weight = null) where T : class
            => SendToAddressAsync<T>(BaseAddress, definition, uriParameters, bodyParameters, cancellationToken, weight);

        internal async Task<WebCallResult<T>> SendToAddressAsync<T>(string baseAddress, RequestDefinition definition, ParameterCollection? uriParameters, ParameterCollection? bodyParameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<T>(baseAddress, definition, uriParameters, bodyParameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        protected override Error ParseErrorResponse(int httpStatusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, IMessageAccessor accessor)
        {
            if (!accessor.IsJson)
                return new ServerError(accessor.GetOriginalString());

            var code = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
            var msg = accessor.GetValue<string>(MessagePath.Get().Property("message"));
            var errors = accessor.GetValue<Dictionary<string, IEnumerable<string>>?>(MessagePath.Get().Property("errors"));
            if (errors == null || !errors.Any())
            {
                if (msg == null)
                    return new ServerError(accessor.GetOriginalString());

                if (code == null)
                    return new ServerError(msg);

                return new ServerError(code.Value, msg);
            }
            else
            {
                return new ServerError(code!.Value, string.Join(", ", errors.Select(x => $"Error field '{x.Key}': {string.Join(" & ", x.Value)}")));
            }
        }

        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
            => ExchangeData.GetServerTimeAsync();

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo()
            => new TimeSyncInfo(_logger, ApiOptions.AutoTimestamp ?? ClientOptions.AutoTimestamp, ApiOptions.TimestampRecalculationInterval ?? ClientOptions.TimestampRecalculationInterval, _timeSyncState);

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset()
            => _timeSyncState.TimeOffset;

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverDate = null)
            => WhiteBitExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverDate);

        /// <inheritdoc />
        public IWhiteBitRestClientV4ApiShared SharedClient => this;

    }
}

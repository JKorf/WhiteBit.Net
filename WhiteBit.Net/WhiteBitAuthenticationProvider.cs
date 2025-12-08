using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net
{
    internal class WhiteBitAuthenticationProvider : AuthenticationProvider
    {
        private static readonly IMessageSerializer _serializer = new SystemTextJsonMessageSerializer(WhiteBitExchange._serializerContext);
        private readonly INonceProvider _nonceProvider;

        public WhiteBitAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            _nonceProvider = nonceProvider ?? new WhiteBitNonceProvider();
        }

        public override void ProcessRequest(RestApiClient apiClient, RestRequestConfiguration request)
        {
            if (!request.Authenticated)
                return;

            var nonce = _nonceProvider.GetNonce().ToString();
            request.BodyParameters ??= new Dictionary<string, object>();
            request.BodyParameters.Add("request", request.Path);
            request.BodyParameters.Add("nonce", nonce);
            request.BodyParameters.Add("nonceWindow", ((WhiteBitRestOptions)apiClient.ClientOptions).EnableNonceWindow);
            request.Headers ??= new Dictionary<string, string>();
            request.Headers.Add("X-TXC-APIKEY", ApiKey);

            var payload = GetSerializedBody(_serializer, request.BodyParameters);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

            request.Headers.Add("X-TXC-PAYLOAD", encoded);
            request.Headers.Add("X-TXC-SIGNATURE", SignHMACSHA512(encoded, SignOutputType.Hex).ToLower());
        }
    }
}

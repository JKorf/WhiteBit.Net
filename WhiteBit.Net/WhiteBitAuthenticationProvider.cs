using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Objects.Options;

namespace WhiteBit.Net
{
    internal class WhiteBitAuthenticationProvider : AuthenticationProvider
    {
        private static IMessageSerializer _serializer = new SystemTextJsonMessageSerializer();

        public WhiteBitAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
        }

        public override void AuthenticateRequest(
            RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            ref IDictionary<string, object>? uriParameters,
            ref IDictionary<string, object>? bodyParameters,
            ref Dictionary<string, string>? headers,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            RequestBodyFormat requestBodyFormat)
        {
            headers = new Dictionary<string, string>() { };

            if (!auth)
                return;

            var nonce = GetMillisecondTimestamp(apiClient);
            bodyParameters ??= new Dictionary<string, object>();
            bodyParameters.Add("request", uri.AbsolutePath);
            bodyParameters.Add("nonce", nonce);
            bodyParameters.Add("nonceWindow", ((WhiteBitRestOptions)apiClient.ClientOptions).EnableNonceWindow);
            headers.Add("X-TXC-APIKEY", ApiKey);

            var payload = GetSerializedBody(_serializer, bodyParameters);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

            headers.Add("X-TXC-PAYLOAD", encoded);
            headers.Add("X-TXC-SIGNATURE", SignHMACSHA512(encoded, SignOutputType.Hex).ToLower());
        }
    }
}

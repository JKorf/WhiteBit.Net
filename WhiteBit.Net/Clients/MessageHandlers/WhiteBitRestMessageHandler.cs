using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters.SystemTextJson.MessageConverters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhiteBit.Net;

namespace WhiteBit.Net.Clients.MessageHandlers
{
    internal class WhiteBitRestMessageHandler : JsonRestMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = WhiteBitExchange._serializerContext;

        public override async ValueTask<Error> ParseErrorResponse(int httpStatusCode, object? state, HttpResponseHeaders responseHeaders, Stream responseStream)
        {
            if (httpStatusCode == 401)
                return new ServerError(new ErrorInfo(ErrorType.Unauthorized, "Unauthorized"));

            var (parseError, document) = await GetJsonDocument(responseStream, state).ConfigureAwait(false);
            if (parseError != null)
                return parseError;

            int? code = document!.RootElement.TryGetProperty("code", out var codeProp) ? codeProp.GetInt32() : null;
            var msg = document!.RootElement.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : null;
            var errors = document.RootElement.TryGetProperty("errors", out var errorsProp) ? errorsProp.Deserialize<Dictionary<string, string[]>>(Options) : null;
            if (errors == null || !errors.Any())
            {
                if (msg == null)
                    return new ServerError(ErrorInfo.Unknown);

                if (code == null)
                    return new ServerError(ErrorInfo.Unknown with { Message = msg });

                if (httpStatusCode == 401 && code == 0)
                    return new ServerError(new ErrorInfo(ErrorType.Unauthorized, "Unauthorized") { Message = msg });

                return new ServerError(ErrorInfo.Unknown with { Message = msg });
            }
            else
            {
                return new ServerError(ErrorInfo.Unknown with { Message = string.Join(", ", errors.Select(x => $"Error field '{x.Key}': {string.Join(" & ", x.Value)}")) } );
            }
        }
    }
}

using CryptoExchange.Net.Objects.Errors;

namespace WhiteBit.Net
{
    internal static class WhiteBitErrors
    {
        // API error responses are not easily parsable. For example response code 0 can mean an invalid API key or Order value too low
        public static ErrorMapping RestErrors { get; } = new ErrorMapping([],
            [
                new ErrorEvaluator("2", (code, msg) => {

                    if (msg?.StartsWith("This action is unauthorized") == true)
                        return new ErrorInfo(ErrorType.Unauthorized, false, "Unauthorized", code);

                    return ErrorInfo.Unknown;
                })
            ]
            );

        public static ErrorMapping SocketErrors { get; } = new ErrorMapping([],

            [
                new ErrorEvaluator("9", (code, msg) => {

                    if (msg?.StartsWith("market") == true && msg.EndsWith("does not exist"))
                        return new ErrorInfo(ErrorType.UnknownSymbol, false, "Unknown symbol", code);

                    return ErrorInfo.Unknown;
                }),
                new ErrorEvaluator("6", (code, msg) => {

                    if (msg?.Equals("require authentication") == true)
                        return new ErrorInfo(ErrorType.Unauthorized, false, "Unauthorized", code);

                    return ErrorInfo.Unknown;
                })
            ]
            );
    }
}

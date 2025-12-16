using CryptoExchange.Net.Objects.Errors;

namespace WhiteBit.Net
{
    internal static class WhiteBitErrors
    {
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

using CryptoExchange.Net.Objects.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

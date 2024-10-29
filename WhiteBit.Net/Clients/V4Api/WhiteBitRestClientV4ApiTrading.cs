using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using WhiteBit.Net.Interfaces.Clients.V4Api;

namespace WhiteBit.Net.Clients.V4Api
{
    /// <inheritdoc />
    internal class WhiteBitRestClientV4ApiTrading : IWhiteBitRestClientV4ApiTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly WhiteBitRestClientV4Api _baseClient;
        private readonly ILogger _logger;

        internal WhiteBitRestClientV4ApiTrading(ILogger logger, WhiteBitRestClientV4Api baseClient)
        {
            _baseClient = baseClient;
            _logger = logger;
        }
    }
}

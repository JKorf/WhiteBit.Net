using CryptoExchange.Net.SharedApis;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// Shared interface for V4 API socket API usage
    /// </summary>
    public interface IWhiteBitSocketClientV4ApiShared :
        IBalanceSocketClient,
        IBookTickerSocketClient,
        IKlineSocketClient,
        ITickerSocketClient,
        ITradeSocketClient,
        IUserTradeSocketClient,
        ISpotOrderSocketClient,
        IPositionSocketClient,
        IFuturesOrderSocketClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface IWhiteBitSocketClientV4SharedApi :
        ISubscribeBalancesOperation,
        ISubscribeBookTickerOperation,
        ISubscribeKlinesOperation,
        ISubscribeTickerOperation,
        ISubscribeTradesOperation,
        ISubscribeUserTradesOperation,
        ISubscribeSpotOrdersOperation,
        ISubscribePositionsOperation,
        ISubscribeFuturesOrdersOperation
    {

    }
}

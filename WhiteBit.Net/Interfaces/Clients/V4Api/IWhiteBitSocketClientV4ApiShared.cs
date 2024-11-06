using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Text;

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
}

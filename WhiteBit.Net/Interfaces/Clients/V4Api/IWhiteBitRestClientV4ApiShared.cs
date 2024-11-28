using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Interfaces.Clients.V4Api
{
    /// <summary>
    /// Shared interface for V4 rest API usage
    /// </summary>
    public interface IWhiteBitRestClientV4ApiShared :
        ISpotSymbolRestClient,
        ISpotTickerRestClient,
        IRecentTradeRestClient,
        IOrderBookRestClient,
        IBalanceRestClient,
        IAssetsRestClient,
        IDepositRestClient,
        IWithdrawalRestClient,
        IWithdrawRestClient,
        ISpotOrderRestClient,
        IFuturesSymbolRestClient,
        IFuturesTickerRestClient,
        ILeverageRestClient,
        IOpenInterestRestClient,
        IPositionHistoryRestClient,
        IFuturesOrderRestClient,
        IFeeRestClient
    {
    }
}

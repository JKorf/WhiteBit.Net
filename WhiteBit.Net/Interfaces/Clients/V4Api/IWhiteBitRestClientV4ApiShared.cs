using CryptoExchange.Net.SharedApis;

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
        IFeeRestClient,
        ISpotTriggerOrderRestClient,
        IFuturesTriggerOrderRestClient,
        IFuturesTpSlRestClient,
        IBookTickerRestClient,
        IFundingRateRestClient,
        ITransferRestClient
    {
    }
}

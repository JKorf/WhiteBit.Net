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

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface IWhiteBitRestClientV4SharedApi :
        IGetSpotSymbolsEndpoint,
        IGetSpotTickerEndpoint,
        IGetAllSpotTickersEndpoint,
        IGetRecentTradesEndpoint,
        IGetOrderBookEndpoint,
        IGetBalancesEndpoint,
        IGetAssetEndpoint,
        IGetAllAssetsEndpoint,
        IGetDepositAddressesEndpoint,
        IGetDepositHistoryEndpoint,
        IGetWithdrawalHistoryEndpoint,
        IWithdrawEndpoint,
        IPlaceSpotOrderEndpoint,
        IGetSpotOrderEndpoint,
        IGetOpenSpotOrdersEndpoint,
        IGetClosedSpotOrdersEndpoint,
        IGetSpotOrderTradesEndpoint,
        IGetSpotUserTradeHistoryEndpoint,
        ICancelSpotOrderEndpoint,
        IGetFuturesSymbolsEndpoint,
        IGetFuturesTickerEndpoint,
        IGetAllFuturesTickersEndpoint,
        IGetLeverageEndpoint,
        ISetLeverageEndpoint,
        IGetOpenInterestEndpoint,
        IGetPositionHistoryEndpoint,
        IPlaceFuturesOrderEndpoint,
        IGetFuturesOrderEndpoint,
        IGetOpenFuturesOrdersEndpoint,
        IGetClosedFuturesOrdersEndpoint,
        IGetFuturesOrderTradesEndpoint,
        IGetFuturesUserTradeHistoryEndpoint,
        ICancelFuturesOrderEndpoint,
        IGetPositionsEndpoint,
        IClosePositionEndpoint,
        IGetFeesEndpoint,
        IPlaceSpotTriggerOrderEndpoint,
        IGetSpotTriggerOrderEndpoint,
        ICancelSpotTriggerOrderEndpoint,
        IPlaceFuturesTriggerOrderEndpoint,
        IGetFuturesTriggerOrderEndpoint,
        ICancelFuturesTriggerOrderEndpoint,
        ISetFuturesTpSlEndpoint,
        ICancelFuturesTpSlEndpoint,
        IGetBookTickerEndpoint,
        IGetFundingRateHistoryEndpoint,
        ITransferEndpoint
    {
    }
}

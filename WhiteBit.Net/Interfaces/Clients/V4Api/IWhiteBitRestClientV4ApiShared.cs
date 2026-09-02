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
        IGetSpotSymbolsRest,
        IGetSpotTickerRest,
        IGetAllSpotTickersRest,
        IGetRecentTradesRest,
        IGetOrderBookRest,
        IGetBalancesRest,
        IGetAssetRest,
        IGetAllAssetsRest,
        IGetDepositAddressesRest,
        IGetDepositHistoryRest,
        IGetWithdrawalHistoryRest,
        IWithdrawRest,
        IPlaceSpotOrderRest,
        IGetSpotOrderRest,
        IGetOpenSpotOrdersRest,
        IGetClosedSpotOrdersRest,
        IGetSpotOrderTradesRest,
        IGetSpotUserTradeHistoryRest,
        ICancelSpotOrderRest,
        IGetFuturesSymbolsRest,
        IGetFuturesTickerRest,
        IGetAllFuturesTickersRest,
        IGetLeverageRest,
        ISetLeverageRest,
        IGetOpenInterestRest,
        IGetPositionHistoryRest,
        IPlaceFuturesOrderRest,
        IGetFuturesOrderRest,
        IGetOpenFuturesOrdersRest,
        IGetClosedFuturesOrdersRest,
        IGetFuturesOrderTradesRest,
        IGetFuturesUserTradeHistoryRest,
        ICancelFuturesOrderRest,
        IGetPositionsRest,
        IClosePositionRest,
        IGetFeesRest,
        IPlaceSpotTriggerOrderRest,
        IGetSpotTriggerOrderRest,
        ICancelSpotTriggerOrderRest,
        IPlaceFuturesTriggerOrderRest,
        IGetFuturesTriggerOrderRest,
        ICancelFuturesTriggerOrderRest,
        ISetFuturesTpSlRest,
        ICancelFuturesTpSlRest,
        IGetBookTickerRest,
        IGetFundingRateHistoryRest,
        ITransferRest
    {
    }
}

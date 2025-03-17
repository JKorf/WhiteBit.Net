using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhiteBit.Net.Objects.Internal;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Converters
{  

    // End manual defined attributes

    [JsonSerializable(typeof(WhiteBitSocketUpdate<Dictionary<string, WhiteBitTradeBalance>[]>))]
    [JsonSerializable(typeof(WhiteBitResponse<string[]>))]
    [JsonSerializable(typeof(WhiteBitResponse<WhiteBitFuturesSymbol[]>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitClosedOrder[]>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitUserTrade[]>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitClosedOrder[]>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitKlineUpdate[]>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitMarginBalance[]>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitMainBalance>))]
    [JsonSerializable(typeof(Dictionary<string, decimal>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitTicker>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitAsset>))]
    [JsonSerializable(typeof(Dictionary<string, WhiteBitSubBalances>))]
    [JsonSerializable(typeof(WhiteBitOffsetResult<WhiteBitUserTrade>))]
    [JsonSerializable(typeof(WhiteBitSocketResponse<WhiteBitSubscribeResponse>))]
    [JsonSerializable(typeof(WhiteBitSocketResponse<object>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitBookUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitTradeUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitLastPriceUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitTickerUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitOrderUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitPositionsUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitBorrow>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitAccountMarginPositionUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitAccountBorrowUpdate>))]
    [JsonSerializable(typeof(WhiteBitSocketUpdate<WhiteBitUserTradeUpdate>))]
    [JsonSerializable(typeof(Dictionary<string, string[]>))]
    [JsonSerializable(typeof(IDictionary<string, object>))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalSetting[]))]
    [JsonSerializable(typeof(WhiteBitCollateralSummary[]))]
    [JsonSerializable(typeof(WhiteBitPosition[]))]
    [JsonSerializable(typeof(WhiteBitPositionHistory[]))]
    [JsonSerializable(typeof(WhiteBitSymbol[]))]
    [JsonSerializable(typeof(WhiteBitTrade[]))]
    [JsonSerializable(typeof(WhiteBitOrderResponse[]))]
    [JsonSerializable(typeof(WhiteBitOrder[]))]
    [JsonSerializable(typeof(WhiteBitUserTrade[]))]
    [JsonSerializable(typeof(WhiteBitKillSwitch[]))]
    [JsonSerializable(typeof(WhiteBitSocketRequest[]))]
    [JsonSerializable(typeof(WhiteBitSocketError[]))]
    [JsonSerializable(typeof(WhiteBitSubscribeResponse[]))]
    [JsonSerializable(typeof(WhiteBitAccountBorrowUpdate[]))]
    [JsonSerializable(typeof(WhiteBitAccountMarginPositionUpdate[]))]
    [JsonSerializable(typeof(WhiteBitAsset[]))]
    [JsonSerializable(typeof(WhiteBitAssetNetworks[]))]
    [JsonSerializable(typeof(WhiteBitAssetLimits[]))]
    [JsonSerializable(typeof(WhiteBitAssetLimit[]))]
    [JsonSerializable(typeof(WhiteBitBookUpdate[]))]
    [JsonSerializable(typeof(WhiteBitBorrows[]))]
    [JsonSerializable(typeof(WhiteBitBorrow[]))]
    [JsonSerializable(typeof(WhiteBitCode[]))]
    [JsonSerializable(typeof(WhiteBitCodeResult[]))]
    [JsonSerializable(typeof(WhiteBitCollateralAccountSummary[]))]
    [JsonSerializable(typeof(WhiteBitCollateralOrder[]))]
    [JsonSerializable(typeof(WhiteBitCollateralOrderConfig[]))]
    [JsonSerializable(typeof(WhiteBitConditionalOrdersResult[]))]
    [JsonSerializable(typeof(WhiteBitConditionalOrders[]))]
    [JsonSerializable(typeof(WhiteBitConvertEstimate[]))]
    [JsonSerializable(typeof(WhiteBitConvertHistory[]))]
    [JsonSerializable(typeof(WhiteBitConvertHistoryEntry[]))]
    [JsonSerializable(typeof(WhiteBitConvertHistoryEntryPath[]))]
    [JsonSerializable(typeof(WhiteBitConvertResult[]))]
    [JsonSerializable(typeof(WhiteBitDepositAddressInfo[]))]
    [JsonSerializable(typeof(WhiteBitDepositAddress[]))]
    [JsonSerializable(typeof(WhiteBitDepositUrl[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawals[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawal[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalDetails[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalPartial[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalConfirmations[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalSettingDetails[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdraw[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawInfo[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalCrypto[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawalFiat[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawDetails[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawProviderDetails[]))]
    [JsonSerializable(typeof(WhiteBitDepositWithdrawInfoFlex[]))]
    [JsonSerializable(typeof(WhiteBitError[]))]
    [JsonSerializable(typeof(WhiteBitGeneratedCodes[]))]
    [JsonSerializable(typeof(WhiteBitGeneratedCode[]))]
    [JsonSerializable(typeof(WhiteBitLastPriceUpdate[]))]
    [JsonSerializable(typeof(WhiteBitLeverage[]))]
    [JsonSerializable(typeof(WhiteBitMainBalance[]))]
    [JsonSerializable(typeof(WhiteBitMiningRewards[]))]
    [JsonSerializable(typeof(WhiteBitMiningReward[]))]
    [JsonSerializable(typeof(WhiteBitOcoOrder[]))]
    [JsonSerializable(typeof(WhiteBitOrders[]))]
    [JsonSerializable(typeof(WhiteBitConditionalOrder[]))]
    [JsonSerializable(typeof(WhiteBitClosedOrders[]))]
    [JsonSerializable(typeof(WhiteBitOrderBook[]))]
    [JsonSerializable(typeof(WhiteBitOrderBookEntry[]))]
    [JsonSerializable(typeof(WhiteBitOrderRequest[]))]
    [JsonSerializable(typeof(WhiteBitOrderUpdate[]))]
    [JsonSerializable(typeof(WhiteBitPositionTpSl[]))]
    [JsonSerializable(typeof(WhiteBitPositionHistoryOrder[]))]
    [JsonSerializable(typeof(WhiteBitPositionsUpdate[]))]
    [JsonSerializable(typeof(WhiteBitPositionUpdate[]))]
    [JsonSerializable(typeof(WhiteBitSocketTicker[]))]
    [JsonSerializable(typeof(WhiteBitSocketTrade[]))]
    [JsonSerializable(typeof(WhiteBitSubAccount[]))]
    [JsonSerializable(typeof(WhiteBitSubAccountKyc[]))]
    [JsonSerializable(typeof(WhiteBitSubAccountPermissions[]))]
    [JsonSerializable(typeof(WhiteBitSubAccounts[]))]
    [JsonSerializable(typeof(WhiteBitSubaccountTransferHistory[]))]
    [JsonSerializable(typeof(WhiteBitSubaccountTransferEntry[]))]
    [JsonSerializable(typeof(WhiteBitSubBalances[]))]
    [JsonSerializable(typeof(WhiteBitSystemStatus[]))]
    [JsonSerializable(typeof(WhiteBitTicker[]))]
    [JsonSerializable(typeof(WhiteBitTickerUpdate[]))]
    [JsonSerializable(typeof(WhiteBitTime[]))]
    [JsonSerializable(typeof(WhiteBitToken[]))]
    [JsonSerializable(typeof(WhiteBitTradeBalance[]))]
    [JsonSerializable(typeof(WhiteBitTradeUpdate[]))]
    [JsonSerializable(typeof(WhiteBitTradingFee[]))]
    [JsonSerializable(typeof(WhiteBitTradingFees[]))]
    [JsonSerializable(typeof(WhiteBitUserTrades[]))]
    [JsonSerializable(typeof(WhiteBitUserTradeUpdate[]))]
    [JsonSerializable(typeof(WhiteBitSocketRequest))]
    [JsonSerializable(typeof(int?))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(long?))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(decimal?))]
    [JsonSerializable(typeof(decimal))]
    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(DateTime?))]
    internal partial class WhiteBitSourceGenerationContext : JsonSerializerContext
    {
    }
}

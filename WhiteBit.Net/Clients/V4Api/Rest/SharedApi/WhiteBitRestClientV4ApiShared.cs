using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhiteBit.Net.Enums;
using WhiteBit.Net.ExtensionMethods;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using WhiteBit.Net.Objects.Models;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitRestClientV4SharedApi :
        SharedApiBase,
        IWhiteBitRestClientV4ApiShared,
        IWhiteBitRestClientV4SharedApi
    {
        private readonly WhiteBitRestClientV4Api _api;
        private const string _exchange = "WhiteBit";
        private const string _topicSpotId = "WhiteBitSpot";
        private const string _topicFuturesId = "WhiteBitFutures";
        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(WhiteBitExchange.Metadata, this);

        private static readonly HashSet<string> _exchangeFiats = ["UAH", "EUR", "USD", "TRY", "GBP", "PLN", "BGN", "CZK", "KZT"];

        public WhiteBitRestClientV4SharedApi(WhiteBitRestClientV4Api api)
            : base(
                  SharedTransport.Rest,
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                GetSpotSymbolsOptions,
                GetSpotTickerOptions,
                GetAllSpotTickersOptions,
                GetBookTickerOptions,
                GetRecentTradesOptions,
                GetOrderBookOptions,
                GetBalancesOptions,
                GetAssetOptions,
                GetAllAssetsOptions,
                GetDepositAddressesOptions,
                GetDepositHistoryOptions,
                GetWithdrawalHistoryOptions,
                WithdrawOptions,
                PlaceSpotOrderOptions,
                GetSpotOrderOptions,
                GetOpenSpotOrdersOptions,
                GetClosedSpotOrdersOptions,
                GetSpotOrderTradesOptions,
                GetSpotUserTradeHistoryOptions,
                CancelSpotOrderOptions,
                GetFuturesSymbolsOptions,
                GetFuturesTickerOptions,
                GetAllFuturesTickersOptions,
                GetLeverageOptions,
                SetLeverageOptions,
                GetOpenInterestOptions,
                GetPositionHistoryOptions,
                PlaceFuturesOrderOptions,
                GetFuturesOrderOptions,
                GetOpenFuturesOrdersOptions,
                GetClosedFuturesOrdersOptions,
                GetFuturesOrderTradesOptions,
                GetFuturesUserTradeHistoryOptions,
                CancelFuturesOrderOptions,
                GetPositionsOptions,
                ClosePositionOptions,
                GetFeeOptions,
                PlaceSpotTriggerOrderOptions,
                GetSpotTriggerOrderOptions,
                CancelSpotTriggerOrderOptions,
                PlaceFuturesTriggerOrderOptions,
                GetFuturesTriggerOrderOptions,
                CancelFuturesTriggerOrderOptions,
                SetFuturesTpSlOptions,
                CancelFuturesTpSlOptions,
                GetFundingRateHistoryOptions,
                TransferOptions
                );
        }

    }
}

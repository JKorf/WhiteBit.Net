using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using CryptoExchange.Net.Objects;
using System.Linq;
using WhiteBit.Net.Objects.Models;
using WhiteBit.Net.Enums;
using System.Xml.Linq;
using CryptoExchange.Net;
using System.Drawing;
using CryptoExchange.Net.Objects.Errors;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitRestClientV4Api : IWhiteBitRestClientV4ApiShared
    {
        private const string _topicSpotId = "WhiteBitSpot";
        private const string _topicFuturesId = "WhiteBitFutures";

        public string Exchange => "WhiteBit";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public TradingMode[] SupportedFuturesModes => new[] { TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Spot Symbol client
        EndpointOptions<GetSymbolsRequest> ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest>(false);

        async Task<ExchangeWebResult<SharedSpotSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotSymbolRestClient)this).GetSpotSymbolsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotSymbol[]>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotSymbol[]>(Exchange, null, default);

            var data = result.Data.Where(x => x.SymbolType == Enums.SymbolType.Spot);

            var response = result.AsExchangeResult<SharedSpotSymbol[]>(Exchange, TradingMode.Spot, 
                data.Select(s => new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name, s.TradingEnabled)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                MinNotionalValue = s.MinOrderValue,
                QuantityDecimals = s.BaseAssetPrecision,
                PriceDecimals = s.QuoteAssetPrecision
            }).ToArray());

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, response.Data);
            return response;
        }

        #endregion

        #region Ticker client

        EndpointOptions<GetTickerRequest> ISpotTickerRestClient.GetSpotTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotTicker>(Exchange, null, default);

            var ticker = result.Data.SingleOrDefault(x => x.Symbol == request.Symbol!.GetSymbol(FormatSymbol));
            if (ticker == null)
                return result.AsExchangeError<SharedSpotTicker>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicSpotId, ticker.Symbol), ticker.Symbol, ticker.LastPrice, null, null, ticker.BaseVolume, ticker.ChangePercentage)
            {
                QuoteVolume = ticker.QuoteVolume
            });
        }

        EndpointOptions<GetTickersRequest> ISpotTickerRestClient.GetSpotTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickersOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotTicker[]>(Exchange, null, default);

            var data = result.Data.Where(x => !x.Symbol.EndsWith("_PERP"));
            return result.AsExchangeResult<SharedSpotTicker[]>(Exchange, TradingMode.Spot, data.Select(x => new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicSpotId, x.Symbol), x.Symbol, x.LastPrice, null, null, x.BaseVolume, x.ChangePercentage)
            {
                QuoteVolume = x.QuoteVolume
            }).ToArray());
        }

        #endregion

        #region Book Ticker client

        EndpointOptions<GetBookTickerRequest> IBookTickerRestClient.GetBookTickerOptions { get; } = new EndpointOptions<GetBookTickerRequest>(false);
        async Task<ExchangeWebResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = ((IBookTickerRestClient)this).GetBookTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedBookTicker>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetOrderBookAsync(request.Symbol!.GetSymbol(FormatSymbol), 1, ct: ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedBookTicker>(Exchange, null, default);

            return resultTicker.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedBookTicker(
                ExchangeSymbolCache.ParseSymbol(request.Symbol.TradingMode == TradingMode.Spot ? _topicSpotId : _topicFuturesId, resultTicker.Data.Symbol),
                resultTicker.Data.Symbol,
                resultTicker.Data.Asks[0].Price,
                resultTicker.Data.Asks[0].Quantity,
                resultTicker.Data.Bids[0].Price,
                resultTicker.Data.Bids[0].Quantity));
        }

        #endregion

        #region Recent Trades client
        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(100, false);

        async Task<ExchangeWebResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IRecentTradeRestClient)this).GetRecentTradesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedTrade[]>(Exchange, validationError);

            // Get data
            var result = await ExchangeData.GetRecentTradesAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedTrade[]>(Exchange, null, default);

            IEnumerable<WhiteBitTrade> data = result.Data;
            if (request.Limit != null)
                data = data.Take(request.Limit.Value);

            // Return
            return result.AsExchangeResult<SharedTrade[]>(Exchange, request.Symbol.TradingMode, data.Select(x => new SharedTrade(x.BaseVolume, x.Price, x.Timestamp)
            {
                Side = x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }
        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(1, 100, false);
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedOrderBook(result.Data.Asks, result.Data.Bids));
        }

        #endregion

        #region Balance Client
        EndpointOptions<GetBalancesRequest> IBalanceRestClient.GetBalancesOptions { get; } = new EndpointOptions<GetBalancesRequest>(true);

        async Task<ExchangeWebResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = ((IBalanceRestClient)this).GetBalancesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedBalance[]>(Exchange, validationError);

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var result = await Account.GetSpotBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result)
                    return result.AsExchangeResult<SharedBalance[]>(Exchange, null, default);

                return result.AsExchangeResult<SharedBalance[]>(Exchange, TradingMode.Spot, result.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)).ToArray());
            }
            else
            {
                var result = await Account.GetCollateralBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result)
                    return result.AsExchangeResult<SharedBalance[]>(Exchange, null, default);

                return result.AsExchangeResult<SharedBalance[]>(Exchange, SupportedFuturesModes, result.Data.Select(x => new SharedBalance(x.Key, x.Value, x.Value)).ToArray());
            }
        }

        #endregion

        #region Asset client
        EndpointOptions<GetAssetsRequest> IAssetsRestClient.GetAssetsOptions { get; } = new EndpointOptions<GetAssetsRequest>(true);

        async Task<ExchangeWebResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = ((IAssetsRestClient)this).GetAssetsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedAsset[]>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets)
                return assets.AsExchangeResult<SharedAsset[]>(Exchange, null, default);

            return assets.AsExchangeResult<SharedAsset[]>(Exchange, TradingMode.Spot, assets.Data.Select(x => 
            {
                var networks = x.Networks.Withdraws.Intersect(x.Networks.Deposits);
                return new SharedAsset(x.Asset)
                {
                    FullName = x.Name,
                    Networks = networks.Select(n => new SharedAssetNetwork(n)
                    {
                        MinWithdrawQuantity = x.Limits.Withdraw[n].Min,
                        MinConfirmations = x.Confirmations?[n]
                    }).ToArray()
                };
            }).ToArray());
        }

        EndpointOptions<GetAssetRequest> IAssetsRestClient.GetAssetOptions { get; } = new EndpointOptions<GetAssetRequest>(false);
        async Task<ExchangeWebResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = ((IAssetsRestClient)this).GetAssetOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedAsset>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets)
                return assets.AsExchangeResult<SharedAsset>(Exchange, null, default);

            var asset = assets.Data.SingleOrDefault(x => x.Asset == request.Asset);
            if (asset == null)
                return assets.AsExchangeError<SharedAsset>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownAsset, "Asset not found")));

            var networks = asset.Networks.Withdraws.Intersect(asset.Networks.Deposits);
            return assets.AsExchangeResult(Exchange, TradingMode.Spot, new SharedAsset(asset.Asset)
            {
                FullName = asset.Name,
                Networks = networks.Select(n => new SharedAssetNetwork(n)
                {
                    MinWithdrawQuantity = asset.Limits.Withdraw[n].Min,
                    MinConfirmations = asset.Confirmations[n]
                }).ToArray()
            });
        }

        #endregion

        #region Deposit client

        EndpointOptions<GetDepositAddressesRequest> IDepositRestClient.GetDepositAddressesOptions { get; } = new EndpointOptions<GetDepositAddressesRequest>(true);
        async Task<ExchangeWebResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositAddressesOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedDepositAddress[]>(Exchange, validationError);

            var depositAddresses = await Account.GetDepositAddressAsync(request.Asset, request.Network, ct: ct).ConfigureAwait(false);
            if (!depositAddresses)
                return depositAddresses.AsExchangeResult<SharedDepositAddress[]>(Exchange, null, default);

            return depositAddresses.AsExchangeResult<SharedDepositAddress[]>(Exchange, TradingMode.Spot, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Account.Address)
            {
                Network = request.Network
            }
            });
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(SharedPaginationSupport.Descending, false, 100);
        async Task<ExchangeWebResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedDeposit[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var deposits = await Account.GetDepositWithdrawalHistoryAsync(
                Enums.TransactionType.Deposit,
                request.Asset,
                limit: request.Limit ?? 100,
                offset: offset,
                ct: ct).ConfigureAwait(false);
            if (!deposits)
                return deposits.AsExchangeResult<SharedDeposit[]>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (deposits.Data.Total > deposits.Data.Offset + deposits.Data.Limit)
                nextToken = new OffsetToken(deposits.Data.Offset + request.Limit ?? 100);

            return deposits.AsExchangeResult<SharedDeposit[]>(Exchange, TradingMode.Spot, deposits.Data.Records.Select(x => new SharedDeposit(x.Asset, x.Quantity, x.TransactionStatus == Enums.TransactionStatus.Success, x.CreateTime)
            {
                Confirmations = x.Confirmations?.Actual,
                Network = x.Network,
                TransactionId = x.TransactionId,
                Tag = x.Memo,
                Id = x.UniqueId
            }).ToArray(), nextToken);
        }

        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(SharedPaginationSupport.Descending, false, 100);
        async Task<ExchangeWebResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IWithdrawalRestClient)this).GetWithdrawalsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedWithdrawal[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var withdrawals = await Account.GetDepositWithdrawalHistoryAsync(
                Enums.TransactionType.Withdrawal,
                request.Asset,
                limit: request.Limit ?? 100,
                offset: offset,
                ct: ct).ConfigureAwait(false);
            if (!withdrawals)
                return withdrawals.AsExchangeResult<SharedWithdrawal[]>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (withdrawals.Data.Total > withdrawals.Data.Offset + withdrawals.Data.Limit)
                nextToken = new OffsetToken(withdrawals.Data.Offset + request.Limit ?? 100);

            return withdrawals.AsExchangeResult<SharedWithdrawal[]>(Exchange, TradingMode.Spot, withdrawals.Data.Records.Select(x => new SharedWithdrawal(x.Asset, x.Address, x.Quantity, x.TransactionStatus == Enums.TransactionStatus.Success, x.CreateTime)
            {
                Confirmations = x.Confirmations?.Actual,
                Network = x.Network,
                Tag = x.Memo,
                TransactionId = x.TransactionId,
                Fee = x.Fee,
                Id = x.UniqueId
            }).ToArray());
        }

        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions();
        async Task<ExchangeWebResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = ((IWithdrawRestClient)this).WithdrawOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var id = Guid.NewGuid().ToString();
            // Get data
            var withdrawal = await Account.WithdrawAsync(
                request.Asset,
                request.Quantity,
                request.Address,
                id,
                true,
                network: request.Network,
                memo: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal)
                return withdrawal.AsExchangeResult<SharedId>(Exchange, null, default);

            return withdrawal.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(id));
        }

        #endregion

        #region Spot Order Client

        SharedFeeDeductionType ISpotOrderRestClient.SpotFeeDeductionType => SharedFeeDeductionType.AddToCost;
        SharedFeeAssetType ISpotOrderRestClient.SpotFeeAssetType => SharedFeeAssetType.QuoteAsset;
        SharedOrderType[] ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.QuoteAsset,
                SharedQuantityType.BaseAsset);

        string ISpotOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomString(32);

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions();
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).PlaceSpotOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.TradingMode,
                [TradingMode.Spot],
                ((ISpotOrderRestClient)this).SpotSupportedOrderTypes,
                ((ISpotOrderRestClient)this).SpotSupportedTimeInForce,
                ((ISpotOrderRestClient)this).SpotSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await Trading.PlaceSpotOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit || request.OrderType == SharedOrderType.Limit ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset,
                quoteQuantity: request.Quantity?.QuantityInQuoteAsset,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(result.Data.OrderId.ToString()));
        }

        EndpointOptions<GetOrderRequest> ISpotOrderRestClient.GetSpotOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedSpotOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, openOrder.Symbol), 
                    openOrder.Symbol,
                    openOrder.OrderId.ToString(),
                    ParseOrderType(openOrder.OrderType, openOrder.PostOnly),
                    openOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    SharedOrderStatus.Open,
                    openOrder.CreateTime)
                {
                    ClientOrderId = openOrder.ClientOrderId == string.Empty ? null : openOrder.ClientOrderId,
                    AveragePrice = openOrder.QuantityFilled != 0 ? openOrder.QuoteQuantityFilled / openOrder.QuantityFilled : null,
                    OrderPrice = openOrder.Price == 0 ? null : openOrder.Price,
                    OrderQuantity = new SharedOrderQuantity((openOrder.OrderType == OrderType.Market || openOrder.OrderType == OrderType.StopMarket) && openOrder.OrderSide == OrderSide.Buy ? null : openOrder.Quantity, (openOrder.OrderType == OrderType.Market || openOrder.OrderType == OrderType.StopMarket) && openOrder.OrderSide == OrderSide.Buy ? openOrder.Quantity : null),
                    QuantityFilled = new SharedOrderQuantity(openOrder.QuantityFilled, openOrder.QuoteQuantityFilled),
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee,
                    TriggerPrice = openOrder.TriggerPrice,
                    IsTriggerOrder = openOrder.TriggerPrice > 0
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedSpotOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedSpotOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status == OrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled;

                return closeOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, closedOrder.Symbol),
                    closedOrder.Symbol,
                    closedOrder.OrderId.ToString(),
                    ParseOrderType(closedOrder.OrderType, closedOrder.PostOnly),
                    closedOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    status,
                    closedOrder.CreateTime)
                {
                    ClientOrderId = closedOrder.ClientOrderId == string.Empty ? null : closedOrder.ClientOrderId,
                    AveragePrice = closedOrder.QuantityFilled != 0 ? closedOrder.QuoteQuantityFilled / closedOrder.QuantityFilled : null,
                    OrderPrice = closedOrder.Price == 0 ? null : closedOrder.Price,
                    OrderQuantity = new SharedOrderQuantity((closedOrder.OrderType == OrderType.Market || closedOrder.OrderType == OrderType.StopMarket) && closedOrder.OrderSide == OrderSide.Buy ? null : closedOrder.Quantity, (closedOrder.OrderType == OrderType.Market || closedOrder.OrderType == OrderType.StopMarket) && closedOrder.OrderSide == OrderSide.Buy ? closedOrder.Quantity : null),
                    QuantityFilled = new SharedOrderQuantity(closedOrder.QuantityFilled, closedOrder.QuoteQuantityFilled),
                    TimeInForce = ParseTimeInForce(closedOrder),
                    Fee = closedOrder.Fee,
                    TriggerPrice = closedOrder.TriggerPrice,
                    IsTriggerOrder = closedOrder.TriggerPrice > 0
                });
            }
        }

        EndpointOptions<GetOpenOrdersRequest> ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOpenSpotOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedSpotOrder[]>(Exchange, null, default);

            var data = orders.Data.Where(x => !x.Symbol.EndsWith("_PERP"));

            return orders.AsExchangeResult<SharedSpotOrder[]>(Exchange, TradingMode.Spot, data.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                SharedOrderStatus.Open,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity((x.OrderType == OrderType.Market || x.OrderType == OrderType.StopMarket) && x.OrderSide == OrderSide.Buy ? null : x.Quantity, (x.OrderType == OrderType.Market || x.OrderType == OrderType.StopMarket) && x.OrderSide == OrderSide.Buy ? x.Quantity : null),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled),
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Ascending, true, 100, true);
        async Task<ExchangeWebResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetClosedSpotOrdersOptions.ValidateRequest(Exchange, request, request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedSpotOrder[]>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Where(x => !x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, x.Symbol), 
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status == OrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity((x.OrderType == OrderType.Market || x.OrderType == OrderType.StopMarket) && x.OrderSide == OrderSide.Buy ? null : x.Quantity, (x.OrderType == OrderType.Market || x.OrderType == OrderType.StopMarket) && x.OrderSide == OrderSide.Buy ? x.Quantity : null),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled),
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }));

            return orders.AsExchangeResult<SharedSpotOrder[]>(Exchange, TradingMode.Spot, data.OrderByDescending(x => x.CreateTime).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderTradesOptions.ValidateRequest(Exchange, request, request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, TradingMode.Spot, orders.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.OrderSide == null ? (SharedOrderSide?)null : x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.Time)
            {
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        PaginatedEndpointOptions<GetUserTradesRequest> ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(SharedPaginationSupport.Descending, true, 100, true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotUserTradesOptions.ValidateRequest(Exchange, request, request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetUserTradesAsync(request.Symbol!.GetSymbol(FormatSymbol),
                offset: offset,
                startTime: request.StartTime,
                endTime: request.EndTime,
                limit: request.Limit ?? 100,
                ct: ct
                ).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            // Get next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Select(y => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, y.Symbol),
                y.Symbol,
                y.OrderId.ToString(),
                y.Id.ToString(),
                y.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                y.Quantity,
                y.Price,
                y.Time)
            {
                Fee = y.Fee,
                FeeAsset = y.FeeAsset,
                Role = y.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray();

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, TradingMode.Spot, data, nextToken);
        }

        EndpointOptions<CancelOrderRequest> ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).CancelSpotOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(order.Data.OrderId.ToString()));
        }

        private SharedOrderType ParseOrderType(OrderType type, bool postOnly)
        {
            if (type == OrderType.Market || type == OrderType.CollateralMarket || type == OrderType.StopMarket) return SharedOrderType.Market;
            if (type == OrderType.MarketBase) return SharedOrderType.Market;
            if ((type == OrderType.Limit || type == OrderType.CollateralLimit) && postOnly) return SharedOrderType.LimitMaker;
            if (type == OrderType.Limit || type == OrderType.CollateralLimit || type == OrderType.StopLimit) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(WhiteBitOrder order)
        {
            if (order.ImmediateOrCancel == true)
                return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }

        #endregion

        #region Futures Symbol client

        EndpointOptions<GetSymbolsRequest> IFuturesSymbolRestClient.GetFuturesSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesSymbolRestClient)this).GetFuturesSymbolsOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesSymbol[]>(Exchange, validationError);

            var symbols = ExchangeData.GetSymbolsAsync(ct);
            var futuresSymbols = ExchangeData.GetFuturesSymbolsAsync(ct);
            await Task.WhenAll(symbols, futuresSymbols).ConfigureAwait(false);
            if (!symbols.Result)
                return symbols.Result.AsExchangeResult<SharedFuturesSymbol[]>(Exchange, null, default);
            if (!futuresSymbols.Result)
                return futuresSymbols.Result.AsExchangeResult<SharedFuturesSymbol[]>(Exchange, null, default);

            var response = futuresSymbols.Result.AsExchangeResult<SharedFuturesSymbol[]>(Exchange, request.TradingMode == null ? SupportedTradingModes : new[] { request.TradingMode.Value },
                futuresSymbols.Result.Data.Select(s =>
                {
                    var symbol = symbols.Result.Data.Single(x => x.Name == s.Symbol);
                    return new SharedFuturesSymbol(s.ProductType == ProductType.Perpetual ? TradingMode.PerpetualLinear : TradingMode.DeliveryLinear, s.BaseAsset, s.QuoteAsset, s.Symbol, true)
                    {
                        MinTradeQuantity = symbol.MinOrderQuantity,
                        MinNotionalValue = symbol.MinOrderValue,
                        QuantityDecimals = symbol.BaseAssetPrecision,
                        PriceDecimals = symbol.QuoteAssetPrecision,
                        ContractSize = 1,
                    };
                }).ToArray());

            ExchangeSymbolCache.UpdateSymbolInfo(_topicFuturesId, response.Data);
            return response;
        }

        #endregion

        #region Futures Ticker client

        EndpointOptions<GetTickerRequest> IFuturesTickerRestClient.GetFuturesTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesTicker>> IFuturesTickerRestClient.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTicker>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedFuturesTicker>(Exchange, null, default);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return resultTicker.AsExchangeError<SharedFuturesTicker>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return resultTicker.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTicker(ExchangeSymbolCache.ParseSymbol(_topicFuturesId, ticker.Symbol), ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice, ticker.BaseVolume, null)
            {
                IndexPrice = ticker.IndexPrice,
                FundingRate = ticker.FundingRate,
                NextFundingTime = ticker.NextFundingRateTime
            });
        }

        EndpointOptions<GetTickersRequest> IFuturesTickerRestClient.GetFuturesTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickersOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTicker[]>(Exchange, validationError);

            var resultTickers = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTickers)
                return resultTickers.AsExchangeResult<SharedFuturesTicker[]>(Exchange, null, default);

            return resultTickers.AsExchangeResult<SharedFuturesTicker[]>(Exchange, request.TradingMode == null ? SupportedTradingModes : new[] { request.TradingMode.Value }, resultTickers.Data.Select(x =>
            {
                return new SharedFuturesTicker(ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.BaseVolume, null)
                {
                    IndexPrice = x.IndexPrice,
                    FundingRate = x.FundingRate,
                    NextFundingTime = x.NextFundingRateTime
                };
            }).ToArray());
        }

        #endregion

        #region Leverage client
        SharedLeverageSettingMode ILeverageRestClient.LeverageSettingType => SharedLeverageSettingMode.PerAccount;

        EndpointOptions<GetLeverageRequest> ILeverageRestClient.GetLeverageOptions { get; } = new EndpointOptions<GetLeverageRequest>(true);
        async Task<ExchangeWebResult<SharedLeverage>> ILeverageRestClient.GetLeverageAsync(GetLeverageRequest request, CancellationToken ct)
        {
            var validationError = ((ILeverageRestClient)this).GetLeverageOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Account.GetCollateralAccountSummaryAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol!.TradingMode, new SharedLeverage(result.Data.Leverage));
        }

        SetLeverageOptions ILeverageRestClient.SetLeverageOptions { get; } = new SetLeverageOptions();
        async Task<ExchangeWebResult<SharedLeverage>> ILeverageRestClient.SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = ((ILeverageRestClient)this).SetLeverageOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Account.SetAccountLeverageAsync((int)request.Leverage, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.TradingMode, new SharedLeverage(result.Data.Leverage));
        }
        #endregion

        #region Open Interest client

        EndpointOptions<GetOpenInterestRequest> IOpenInterestRestClient.GetOpenInterestOptions { get; } = new EndpointOptions<GetOpenInterestRequest>(true);
        async Task<ExchangeWebResult<SharedOpenInterest>> IOpenInterestRestClient.GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = ((IOpenInterestRestClient)this).GetOpenInterestOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOpenInterest>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedOpenInterest>(Exchange, null, default);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return resultTicker.AsExchangeError<SharedOpenInterest>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return resultTicker.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedOpenInterest(ticker.OpenInterest));
        }

        #endregion

        #region Position History client

        GetPositionHistoryOptions IPositionHistoryRestClient.GetPositionHistoryOptions { get; } = new GetPositionHistoryOptions(SharedPaginationSupport.Descending, true, 100)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetPositionHistoryRequest.Symbol), typeof(SharedSymbol), "The symbol to get position history for", "ETH_PERP")
            }
        };
        async Task<ExchangeWebResult<SharedPositionHistory[]>> IPositionHistoryRestClient.GetPositionHistoryAsync(GetPositionHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IPositionHistoryRestClient)this).GetPositionHistoryOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedPositionHistory[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var positions = await CollateralTrading.GetPositionHistoryAsync(
                symbol: request.Symbol!.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: request.EndTime,
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct
                ).ConfigureAwait(false);
            if (!positions)
                return positions.AsExchangeResult<SharedPositionHistory[]>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (positions.Data.Count() == (request.Limit ?? 100))
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            return positions.AsExchangeResult<SharedPositionHistory[]>(Exchange, request.Symbol.TradingMode, positions.Data.Select(x => new SharedPositionHistory(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), 
                x.Symbol,
                x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                x.BasePrice,
                x.OrderDetail.BasePrice,
                x.OrderDetail.TradeQuantity,
                x.OrderDetail.RealizedPnl ?? 0,
                x.OpenTime)
            {
                PositionId = x.PositionId.ToString()
            }).ToArray(), nextToken);
        }
        #endregion

        #region Futures Order Client

        SharedFeeDeductionType IFuturesOrderRestClient.FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        SharedFeeAssetType IFuturesOrderRestClient.FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        SharedOrderType[] IFuturesOrderRestClient.FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market };
        SharedTimeInForce[] IFuturesOrderRestClient.FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport IFuturesOrderRestClient.FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string IFuturesOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomString(32);

        PlaceFuturesOrderOptions IFuturesOrderRestClient.PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(true)
        {
            RequestNotes = "ReduceOnly is not supported"
        };
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).PlaceFuturesOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.TradingMode,
                SupportedFuturesModes,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderTypes,
                ((IFuturesOrderRestClient)this).FuturesSupportedTimeInForce,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                takeProfitPrice: request.TakeProfitPrice,
                stopLossPrice: request.StopLossPrice,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.OrderId.ToString()));
        }

        EndpointOptions<GetOrderRequest> IFuturesOrderRestClient.GetFuturesOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder>> IFuturesOrderRestClient.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, openOrder.Symbol), 
                    openOrder.Symbol,
                    openOrder.OrderId.ToString(),
                    ParseOrderType(openOrder.OrderType, openOrder.PostOnly),
                    openOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    SharedOrderStatus.Open,
                    openOrder.CreateTime)
                {
                    ClientOrderId = openOrder.ClientOrderId == string.Empty ? null : openOrder.ClientOrderId,
                    AveragePrice = openOrder.QuantityFilled != 0 ? openOrder.QuoteQuantityFilled / openOrder.QuantityFilled : null,
                    OrderPrice = openOrder.Price == 0 ? null : openOrder.Price,
                    OrderQuantity = new SharedOrderQuantity(openOrder.Quantity, contractQuantity: openOrder.Quantity),
                    QuantityFilled = new SharedOrderQuantity(openOrder.QuantityFilled, openOrder.QuoteQuantityFilled, openOrder.QuantityFilled),
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee,
                    TakeProfitPrice = openOrder.OtoData?.TakeProfit,
                    StopLossPrice = openOrder.OtoData?.StopLoss,
                    TriggerPrice = openOrder.TriggerPrice,
                    IsTriggerOrder = openOrder.TriggerPrice > 0
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedFuturesOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status == OrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled;

                return closeOrders.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, closedOrder.Symbol), 
                    closedOrder.Symbol,
                    closedOrder.OrderId.ToString(),
                    ParseOrderType(closedOrder.OrderType, closedOrder.PostOnly),
                    closedOrder.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    status,
                    closedOrder.CreateTime)
                    {
                        ClientOrderId = closedOrder.ClientOrderId == string.Empty ? null : closedOrder.ClientOrderId,
                        AveragePrice = closedOrder.QuantityFilled != 0 ? closedOrder.QuoteQuantityFilled / closedOrder.QuantityFilled : null,
                        OrderPrice = closedOrder.Price == 0 ? null : closedOrder.Price,
                        OrderQuantity = new SharedOrderQuantity(closedOrder.Quantity, contractQuantity: closedOrder.Quantity),
                        QuantityFilled = new SharedOrderQuantity(closedOrder.QuantityFilled, closedOrder.QuoteQuantityFilled, closedOrder.QuantityFilled),
                        TimeInForce = ParseTimeInForce(closedOrder),
                        Fee = closedOrder.Fee,
                        TakeProfitPrice = closedOrder.OtoData?.TakeProfit,
                        StopLossPrice = closedOrder.OtoData?.StopLoss,
                        TriggerPrice = closedOrder.TriggerPrice,
                        IsTriggerOrder = closedOrder.TriggerPrice > 0
                });
            }            
        }

        EndpointOptions<GetOpenOrdersRequest> IFuturesOrderRestClient.GetOpenFuturesOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetOpenFuturesOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, null, default);

            var data = orders.Data.Where(x => x.Symbol.EndsWith("_PERP"));

            return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, SupportedFuturesModes, data.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                SharedOrderStatus.Open,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled, x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee,
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> IFuturesOrderRestClient.GetClosedFuturesOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Descending, true, 100, true);
        async Task<ExchangeWebResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetClosedFuturesOrdersOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Where(x => x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, xk.Key), 
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status == OrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled, x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee,
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }));

            return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, request.Symbol.TradingMode, data.OrderByDescending(x => x.CreateTime).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> IFuturesOrderRestClient.GetFuturesOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderTradesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, request.TradingMode, orders.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), 
                request.Symbol!.GetSymbol(FormatSymbol),
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.OrderSide == null ? (SharedOrderSide?)null : x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.Time)
            {
                Fee = x.Fee,
                Role = x.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        PaginatedEndpointOptions<GetUserTradesRequest> IFuturesOrderRestClient.GetFuturesUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(SharedPaginationSupport.Descending, true, 100, true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesUserTradesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetUserTradesAsync(request.Symbol!.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct
                ).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            // Get next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Select(y => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, y.Symbol), 
                y.Symbol,
                y.OrderId.ToString(),
                y.Id.ToString(),
                y.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                y.Quantity,
                y.Price,
                y.Time)
            {
                Fee = y.Fee,
                Role = y.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray();

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, TradingMode.PerpetualLinear, data, nextToken);
        }

        EndpointOptions<CancelOrderRequest> IFuturesOrderRestClient.CancelFuturesOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).CancelFuturesOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(order.Data.OrderId.ToString()));
        }

        EndpointOptions<GetPositionsRequest> IFuturesOrderRestClient.GetPositionsOptions { get; } = new EndpointOptions<GetPositionsRequest>(true);
        async Task<ExchangeWebResult<SharedPosition[]>> IFuturesOrderRestClient.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetPositionsOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedPosition[]>(Exchange, validationError);

            var result = await CollateralTrading.GetOpenPositionsAsync(symbol: request.Symbol?.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedPosition[]>(Exchange, null, default);

            var data = result.Data;
            var resultTypes = request.Symbol == null && request.TradingMode == null ? SupportedTradingModes : request.Symbol != null ? new[] { request.Symbol.TradingMode } : new[] { request.TradingMode!.Value };
            return result.AsExchangeResult<SharedPosition[]>(Exchange, resultTypes, data.Select(x =>
            new SharedPosition(ExchangeSymbolCache.ParseSymbol(_topicFuturesId, x.Symbol), x.Symbol, Math.Abs(x.Quantity), x.UpdateTime)
            {
                UnrealizedPnl = x.Pnl,
                LiquidationPrice = x.LiquidationPrice == 0 ? null : x.LiquidationPrice,
                AverageOpenPrice = x.BasePrice,
                PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short, 
                TakeProfitPrice = x.TpSl?.TakeProfitPrice,
                StopLossPrice = x.TpSl?.StopLossPrice
            }).ToArray());
        }

        EndpointOptions<ClosePositionRequest> IFuturesOrderRestClient.ClosePositionOptions { get; } = new EndpointOptions<ClosePositionRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(ClosePositionRequest.PositionSide), typeof(SharedPositionSide), "Current side of the position to close", SharedPositionSide.Long),
                new ParameterDescription(nameof(ClosePositionRequest.Quantity), typeof(decimal), "Quantity of the position is required", 0.1m),
            }
        };
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).ClosePositionOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await CollateralTrading.PlaceOrderAsync(
                symbol,
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                NewOrderType.Market,
                request.Quantity,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.OrderId.ToString()));
        }

        #endregion

        #region Fee Client
        EndpointOptions<GetFeeRequest> IFeeRestClient.GetFeeOptions { get; } = new EndpointOptions<GetFeeRequest>(false);

        async Task<ExchangeWebResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = ((IFeeRestClient)this).GetFeeOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFee>(Exchange, validationError);

            // Get data
            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFee>(Exchange, null, default);

            var symbol = result.Data.SingleOrDefault(x => x.Name == request.Symbol!.GetSymbol(FormatSymbol));
            if (symbol == null)
                return result.AsExchangeError<SharedFee>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            // Return
            return result.AsExchangeResult(Exchange, request.TradingMode, new SharedFee(symbol.MakerFee, symbol.TakerFee));
        }
        #endregion

        #region Spot Trigger Order Client

        PlaceSpotTriggerOrderOptions ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(true)
        {
        };

        async Task<ExchangeWebResult<SharedId>> ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTriggerOrderRestClient)this).PlaceSpotTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes, ((ISpotOrderRestClient)this).SpotSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await Trading.PlaceSpotOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.OrderSide == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                request.OrderPrice == null ? NewOrderType.StopMarket : NewOrderType.StopLimit,
                quantity: request.Quantity.QuantityInBaseAsset,
                quoteQuantity: request.Quantity.QuantityInQuoteAsset,
                triggerPrice: request.TriggerPrice,
                price: request.OrderPrice,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(result.Data.OrderId.ToString()));
        }

        EndpointOptions<GetOrderRequest> ISpotTriggerOrderRestClient.GetSpotTriggerOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true)
        {
        };
        async Task<ExchangeWebResult<SharedSpotTriggerOrder>> ISpotTriggerOrderRestClient.GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTriggerOrderRestClient)this).GetSpotTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotTriggerOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedSpotTriggerOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, openOrder.Symbol),
                    openOrder.Symbol,
                    openOrder.OrderId.ToString(),
                    ParseOrderType(openOrder.OrderType, openOrder.PostOnly),
                    openOrder.OrderSide == OrderSide.Buy ? SharedTriggerOrderDirection.Enter: SharedTriggerOrderDirection.Exit,
                    SharedTriggerOrderStatus.Active,
                    openOrder.TriggerPrice ?? 0,
                    openOrder.CreateTime)
                {
                    PlacedOrderId = openOrder.OrderId.ToString(),
                    AveragePrice = openOrder.QuantityFilled != 0 ? openOrder.QuoteQuantityFilled / openOrder.QuantityFilled : null,
                    OrderPrice = openOrder.Price == 0 ? null : openOrder.Price,
                    OrderQuantity = new SharedOrderQuantity((openOrder.OrderType == OrderType.Market || openOrder.OrderType == OrderType.StopMarket) && openOrder.OrderSide == OrderSide.Buy ? null : openOrder.Quantity, (openOrder.OrderType == OrderType.Market || openOrder.OrderType == OrderType.StopMarket) && openOrder.OrderSide == OrderSide.Buy ? openOrder.Quantity : null),
                    QuantityFilled = new SharedOrderQuantity(openOrder.QuantityFilled, openOrder.QuoteQuantityFilled),
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee,
                    ClientOrderId = openOrder.ClientOrderId
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedSpotTriggerOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedSpotTriggerOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                return closeOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, closedOrder.Symbol),
                    closedOrder.Symbol,
                    closedOrder.OrderId.ToString(),
                    ParseOrderType(closedOrder.OrderType, closedOrder.PostOnly),
                    closedOrder.OrderSide == OrderSide.Buy ? SharedTriggerOrderDirection.Enter: SharedTriggerOrderDirection.Exit,
                    ParseTriggerOrderStatus(closedOrder),
                    closedOrder.TriggerPrice ?? 0,
                    closedOrder.CreateTime)
                {
                    PlacedOrderId = closedOrder.OrderId.ToString(),
                    AveragePrice = closedOrder.QuantityFilled != 0 ? closedOrder.QuoteQuantityFilled / closedOrder.QuantityFilled : null,
                    OrderPrice = closedOrder.Price == 0 ? null : closedOrder.Price,
                    OrderQuantity = new SharedOrderQuantity((closedOrder.OrderType == OrderType.Market || closedOrder.OrderType == OrderType.StopMarket) && closedOrder.OrderSide == OrderSide.Buy ? null : closedOrder.Quantity, (closedOrder.OrderType == OrderType.Market || closedOrder.OrderType == OrderType.StopMarket) && closedOrder.OrderSide == OrderSide.Buy ? closedOrder.Quantity : null),
                    QuantityFilled = new SharedOrderQuantity(closedOrder.QuantityFilled, closedOrder.QuoteQuantityFilled),
                    TimeInForce = ParseTimeInForce(closedOrder),
                    Fee = closedOrder.Fee,
                    ClientOrderId = closedOrder.ClientOrderId
                });
            }
        }

        EndpointOptions<CancelOrderRequest> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTriggerOrderRestClient)this).CancelSpotTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(request.OrderId));
        }

        #endregion

        #region Futures Trigger Order Client
        PlaceFuturesTriggerOrderOptions IFuturesTriggerOrderRestClient.PlaceFuturesTriggerOrderOptions { get; } = new PlaceFuturesTriggerOrderOptions(false)
        {
        };

        async Task<ExchangeWebResult<SharedId>> IFuturesTriggerOrderRestClient.PlaceFuturesTriggerOrderAsync(PlaceFuturesTriggerOrderRequest request, CancellationToken ct)
        {
            var side = GetOrderSide(request);
            var validationError = ((IFuturesTriggerOrderRestClient)this).PlaceFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes, side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell, ((IFuturesOrderRestClient)this).FuturesSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                side,
                request.OrderPrice == null ? NewOrderType.StopMarket : NewOrderType.StopLimit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts,
                triggerPrice: request.TriggerPrice,
                price: request.OrderPrice,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.OrderId.ToString()));
        }

        private OrderSide GetOrderSide(PlaceFuturesTriggerOrderRequest request)
        {
            if (request.PositionSide == SharedPositionSide.Long)
                return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Buy : OrderSide.Sell;
        
            return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Sell : OrderSide.Buy;
        }

        EndpointOptions<GetOrderRequest> IFuturesTriggerOrderRestClient.GetFuturesTriggerOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true)
        {
        };
        async Task<ExchangeWebResult<SharedFuturesTriggerOrder>> IFuturesTriggerOrderRestClient.GetFuturesTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTriggerOrderRestClient)this).GetFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedFuturesTriggerOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedFuturesTriggerOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, openOrder.Symbol),
                    openOrder.Symbol,
                    openOrder.OrderId.ToString(),
                    ParseOrderType(openOrder.OrderType, openOrder.PostOnly),
                    null,
                    SharedTriggerOrderStatus.Active,
                    openOrder.TriggerPrice ?? 0,
                    null,
                    openOrder.CreateTime)
                {
                    PlacedOrderId = openOrder.OrderId.ToString(),
                    AveragePrice = openOrder.QuantityFilled != 0 ? openOrder.QuoteQuantityFilled / openOrder.QuantityFilled : null,
                    OrderPrice = openOrder.Price == 0 ? null : openOrder.Price,
                    OrderQuantity = new SharedOrderQuantity(openOrder.Quantity, contractQuantity: openOrder.Quantity),
                    QuantityFilled = new SharedOrderQuantity(openOrder.QuantityFilled, openOrder.QuoteQuantityFilled, contractQuantity: openOrder.QuantityFilled),
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee,
                    ClientOrderId = openOrder.ClientOrderId
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedFuturesTriggerOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedFuturesTriggerOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                
                return closeOrders.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, closedOrder.Symbol),
                    closedOrder.Symbol,
                    closedOrder.OrderId.ToString(),
                    ParseOrderType(closedOrder.OrderType, closedOrder.PostOnly),
                    closedOrder.OrderSide == OrderSide.Buy ? SharedTriggerOrderDirection.Enter : SharedTriggerOrderDirection.Exit,
                    ParseTriggerOrderStatus(closedOrder),
                    closedOrder.TriggerPrice ?? 0,
                    null,
                    closedOrder.CreateTime)
                {
                    PlacedOrderId = closedOrder.OrderId.ToString(),
                    AveragePrice = closedOrder.QuantityFilled != 0 ? closedOrder.QuoteQuantityFilled / closedOrder.QuantityFilled : null,
                    OrderPrice = closedOrder.Price == 0 ? null : closedOrder.Price,
                    OrderQuantity = new SharedOrderQuantity(closedOrder.Quantity, contractQuantity: closedOrder.Quantity),
                    QuantityFilled = new SharedOrderQuantity(closedOrder.QuantityFilled, closedOrder.QuoteQuantityFilled, contractQuantity: closedOrder.QuantityFilled),
                    TimeInForce = ParseTimeInForce(closedOrder),
                    Fee = closedOrder.Fee
                });
            }
        }

        private SharedTriggerOrderStatus ParseTriggerOrderStatus(WhiteBitClosedOrder closedOrder)
        {
            if (closedOrder.Status == OrderStatus.Open)
                return SharedTriggerOrderStatus.Active;

            if (closedOrder.Status == OrderStatus.Filled)
                return SharedTriggerOrderStatus.Filled;

            return SharedTriggerOrderStatus.CanceledOrRejected;
        }

        EndpointOptions<CancelOrderRequest> IFuturesTriggerOrderRestClient.CancelFuturesTriggerOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesTriggerOrderRestClient.CancelFuturesTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTriggerOrderRestClient)this).CancelFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(request.OrderId));
        }

        #endregion

        #region Tp/SL Client
        EndpointOptions<SetTpSlRequest> IFuturesTpSlRestClient.SetFuturesTpSlOptions { get; } = new EndpointOptions<SetTpSlRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(SetTpSlRequest.Quantity), typeof(decimal), "Quantity of the position to close, required by API", 0.123m)
            }
        };

        async Task<ExchangeWebResult<SharedId>> IFuturesTpSlRestClient.SetFuturesTpSlAsync(SetTpSlRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTpSlRestClient)this).SetFuturesTpSlOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                NewOrderType.StopMarket,
                quantity: request.Quantity!.Value,
                triggerPrice: request.TriggerPrice,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.OrderId.ToString()));
        }

        EndpointOptions<CancelTpSlRequest> IFuturesTpSlRestClient.CancelFuturesTpSlOptions { get; } = new EndpointOptions<CancelTpSlRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(CancelTpSlRequest.OrderId), typeof(string), "Id of the tp/sl order", "123123")
            }
        };

        async Task<ExchangeWebResult<bool>> IFuturesTpSlRestClient.CancelFuturesTpSlAsync(CancelTpSlRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTpSlRestClient)this).CancelFuturesTpSlOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<bool>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<bool>(Exchange, ArgumentError.Invalid(nameof(CancelTpSlRequest.OrderId), "Invalid order id"));

            var result = await Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<bool>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, true);
        }

        #endregion

        #region Funding Rate client
        GetFundingRateHistoryOptions IFundingRateRestClient.GetFundingRateHistoryOptions { get; } = new GetFundingRateHistoryOptions(SharedPaginationSupport.Descending, true, 100, false);

        async Task<ExchangeWebResult<SharedFundingRate[]>> IFundingRateRestClient.GetFundingRateHistoryAsync(GetFundingRateHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFundingRateRestClient)this).GetFundingRateHistoryOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFundingRate[]>(Exchange, validationError);

            DateTime? fromTime = null;
            if (pageToken is DateTimeToken token)
                fromTime = token.LastTime;

            // Get data
            var result = await ExchangeData.GetFundingHistoryAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                startTime: fromTime ?? request.StartTime,
                endTime: request.EndTime,
                limit: request.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFundingRate[]>(Exchange, null, default);

            DateTimeToken? nextToken = null;
            if (result.Data.Count() == (request.Limit ?? 100))
                nextToken = new DateTimeToken(result.Data.Min(x => x.FundingTime).AddSeconds(-1));

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, result.Data.Select(x => new SharedFundingRate(x.FundingRate, x.FundingTime)).ToArray(), nextToken);
        }
        #endregion
    }
}

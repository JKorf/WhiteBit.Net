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

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitRestClientV4Api : IWhiteBitRestClientV4ApiShared
    {
        public string Exchange => "WhiteBit";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public TradingMode[] SupportedFuturesModes => new[] { TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Spot Symbol client
        EndpointOptions<GetSymbolsRequest> ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest>(false);

        async Task<ExchangeWebResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotSymbolRestClient)this).GetSpotSymbolsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotSymbol>>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, null, default);

            var data = result.Data.Where(x => x.SymbolType == Enums.SymbolType.Spot);

            return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, TradingMode.Spot, 
                data.Select(s => new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name, s.TradingEnabled)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                MinNotionalValue = s.MinOrderValue,
                QuantityDecimals = s.BaseAssetPrecision,
                PriceDecimals = s.QuoteAssetPrecision
            }).ToArray());
        }

        #endregion

        #region Ticker client

        EndpointOptions<GetTickerRequest> ISpotTickerRestClient.GetSpotTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotTicker>(Exchange, null, default);

            var ticker = result.Data.SingleOrDefault(x => x.Symbol == request.Symbol.GetSymbol(FormatSymbol));
            if (ticker == null)
                return result.AsExchangeError<SharedSpotTicker>(Exchange, new ServerError("Not found"));

            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotTicker(ticker.Symbol, ticker.LastPrice, null, null, ticker.BaseVolume, ticker.ChangePercentage));
        }

        EndpointOptions<GetTickersRequest> ISpotTickerRestClient.GetSpotTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotTicker>>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickersOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotTicker>>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotTicker>>(Exchange, null, default);

            var data = result.Data.Where(x => !x.Symbol.EndsWith("_PERP"));
            return result.AsExchangeResult<IEnumerable<SharedSpotTicker>>(Exchange, TradingMode.Spot, data.Select(x => new SharedSpotTicker(x.Symbol, x.LastPrice, null, null, x.BaseVolume, x.ChangePercentage)).ToArray());
        }

        #endregion

        #region Recent Trades client
        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(100, false);

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IRecentTradeRestClient)this).GetRecentTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedTrade>>(Exchange, validationError);

            // Get data
            var result = await ExchangeData.GetRecentTradesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, null, default);

            var data = result.Data;
            if (request.Limit != null)
                data = data.Take(request.Limit.Value);

            // Return
            return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, TradingMode.Spot, data.Select(x => new SharedTrade(x.BaseVolume, x.Price, x.Timestamp)
            {
                Side = x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }
        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(1, 100, false);
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedOrderBook(result.Data.Asks, result.Data.Bids));
        }

        #endregion

        #region Balance Client
        EndpointOptions<GetBalancesRequest> IBalanceRestClient.GetBalancesOptions { get; } = new EndpointOptions<GetBalancesRequest>(true);

        async Task<ExchangeWebResult<IEnumerable<SharedBalance>>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = ((IBalanceRestClient)this).GetBalancesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedBalance>>(Exchange, validationError);

            var result = await Account.GetSpotBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, null, default);

            return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, TradingMode.Spot, result.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)).ToArray());
        }

        #endregion

        #region Asset client
        EndpointOptions<GetAssetsRequest> IAssetsRestClient.GetAssetsOptions { get; } = new EndpointOptions<GetAssetsRequest>(true);

        async Task<ExchangeWebResult<IEnumerable<SharedAsset>>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = ((IAssetsRestClient)this).GetAssetsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedAsset>>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets)
                return assets.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);

            return assets.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, TradingMode.Spot, assets.Data.Select(x => 
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
                return assets.AsExchangeError<SharedAsset>(Exchange, new ServerError("Not found"));

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
        async Task<ExchangeWebResult<IEnumerable<SharedDepositAddress>>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositAddressesOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDepositAddress>>(Exchange, validationError);

            var depositAddresses = await Account.GetDepositAddressAsync(request.Asset, request.Network, ct: ct).ConfigureAwait(false);
            if (!depositAddresses)
                return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, null, default);

            return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, TradingMode.Spot, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Account.Address)
            {
                Network = request.Network
            }
            });
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(SharedPaginationSupport.Descending, false, 100);
        async Task<ExchangeWebResult<IEnumerable<SharedDeposit>>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDeposit>>(Exchange, validationError);

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
                return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (deposits.Data.Total > deposits.Data.Offset + deposits.Data.Limit)
                nextToken = new OffsetToken(deposits.Data.Offset + request.Limit ?? 100);

            return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, TradingMode.Spot, deposits.Data.Records.Select(x => new SharedDeposit(x.Asset, x.Quantity, x.TransactionStatus == Enums.TransactionStatus.Success, x.CreateTime)
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
        async Task<ExchangeWebResult<IEnumerable<SharedWithdrawal>>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IWithdrawalRestClient)this).GetWithdrawalsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedWithdrawal>>(Exchange, validationError);

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
                return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (withdrawals.Data.Total > withdrawals.Data.Offset + withdrawals.Data.Limit)
                nextToken = new OffsetToken(withdrawals.Data.Offset + request.Limit ?? 100);

            return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, TradingMode.Spot, withdrawals.Data.Records.Select(x => new SharedWithdrawal(x.Asset, x.Address, x.Quantity, x.TransactionStatus == Enums.TransactionStatus.Success, x.CreateTime)
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
        IEnumerable<SharedOrderType> ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        IEnumerable<SharedTimeInForce> ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAndQuoteAsset,
                SharedQuantityType.BaseAsset);

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions();
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).PlaceSpotOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.Symbol.TradingMode,
                [TradingMode.Spot],
                ((ISpotOrderRestClient)this).SpotSupportedOrderTypes,
                ((ISpotOrderRestClient)this).SpotSupportedTimeInForce,
                ((ISpotOrderRestClient)this).SpotSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await Trading.PlaceSpotOrderAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit || request.OrderType == SharedOrderType.Limit ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity,
                quoteQuantity: request.QuoteQuantity,
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
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, new ArgumentError("Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedSpotOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotOrder(
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
                    Quantity = openOrder.OrderType == OrderType.Market && openOrder.OrderSide == OrderSide.Buy ? null : openOrder.Quantity,
                    QuantityFilled = openOrder.QuantityFilled,
                    QuoteQuantity = openOrder.OrderType == OrderType.Market && openOrder.OrderSide == OrderSide.Buy ? openOrder.Quantity : null,
                    QuoteQuantityFilled = openOrder.QuoteQuantityFilled,
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedSpotOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedSpotOrder>(Exchange, new ServerError("Not found"));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status == ClosedOrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled;

                return closeOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotOrder(
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
                    Quantity = closedOrder.OrderType == OrderType.Market && closedOrder.OrderSide == OrderSide.Buy ? null : closedOrder.Quantity,
                    QuantityFilled = closedOrder.QuantityFilled,
                    QuoteQuantity = closedOrder.OrderType == OrderType.Market && closedOrder.OrderSide == OrderSide.Buy ? closedOrder.Quantity : null,
                    QuoteQuantityFilled = closedOrder.QuoteQuantityFilled,
                    TimeInForce = ParseTimeInForce(closedOrder),
                    Fee = closedOrder.Fee
                });
            }
        }

        EndpointOptions<GetOpenOrdersRequest> ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOpenSpotOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null, default);

            var data = orders.Data.Where(x => !x.Symbol.EndsWith("_PERP"));

            return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, TradingMode.Spot, data.Select(x => new SharedSpotOrder(
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
                Quantity = x.OrderType == OrderType.Market && x.OrderSide == OrderSide.Buy ? null : x.Quantity,
                QuantityFilled = x.QuantityFilled,
                QuoteQuantity = x.OrderType == OrderType.Market && x.OrderSide == OrderSide.Buy ? x.Quantity : null,
                QuoteQuantityFilled = x.QuoteQuantityFilled,
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Ascending, true, 100, true);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetClosedSpotOrdersOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Where(x => !x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedSpotOrder(
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status == ClosedOrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                Quantity = x.OrderType == OrderType.Market && x.OrderSide == OrderSide.Buy ? null : x.Quantity,
                QuantityFilled = x.QuantityFilled,
                QuoteQuantity = x.OrderType == OrderType.Market && x.OrderSide == OrderSide.Buy ? x.Quantity : null,
                QuoteQuantityFilled = x.QuoteQuantityFilled,
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee
            }));

            return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, TradingMode.Spot, data.OrderByDescending(x => x.CreateTime).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, new ArgumentError("Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.Spot, orders.Data.Select(x => new SharedUserTrade(
                request.Symbol.GetSymbol(FormatSymbol),
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

        PaginatedEndpointOptions<GetUserTradesRequest> ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(SharedPaginationSupport.Descending, true, 100, true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotUserTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetUserTradesAsync(request.Symbol.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct
                ).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            // Get next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Select(y => new SharedUserTrade(
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

            return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.Spot, data, nextToken);
        }

        EndpointOptions<CancelOrderRequest> ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).CancelSpotOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, [TradingMode.Spot]);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(order.Data.OrderId.ToString()));
        }

        private SharedOrderType ParseOrderType(OrderType type, bool postOnly)
        {
            if (type == OrderType.Market || type == OrderType.CollateralMarket) return SharedOrderType.Market;
            if (type == OrderType.MarketBase) return SharedOrderType.Market;
            if ((type == OrderType.Limit || type == OrderType.CollateralLimit) && postOnly) return SharedOrderType.LimitMaker;
            if (type == OrderType.Limit || type == OrderType.CollateralLimit) return SharedOrderType.Limit;

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
        async Task<ExchangeWebResult<IEnumerable<SharedFuturesSymbol>>> IFuturesSymbolRestClient.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesSymbolRestClient)this).GetFuturesSymbolsOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedFuturesSymbol>>(Exchange, validationError);

            var symbols = ExchangeData.GetSymbolsAsync(ct);
            var futuresSymbols = ExchangeData.GetFuturesSymbolsAsync(ct);
            await Task.WhenAll(symbols, futuresSymbols).ConfigureAwait(false);
            if (!symbols.Result)
                return symbols.Result.AsExchangeResult<IEnumerable<SharedFuturesSymbol>>(Exchange, null, default);
            if (!futuresSymbols.Result)
                return futuresSymbols.Result.AsExchangeResult<IEnumerable<SharedFuturesSymbol>>(Exchange, null, default);

            return futuresSymbols.Result.AsExchangeResult<IEnumerable<SharedFuturesSymbol>>(Exchange, request.TradingMode == null ? SupportedTradingModes : new[] { request.TradingMode.Value },
                futuresSymbols.Result.Data.Select(s =>
                {
                    var symbol = symbols.Result.Data.Single(x => x.Name == s.Symbol);
                    return new SharedFuturesSymbol(s.ProductType == ProductType.Perpetual ? SharedSymbolType.PerpetualLinear : SharedSymbolType.DeliveryLinear, s.BaseAsset, s.QuoteAsset, s.Symbol, true)
                    {
                        MinTradeQuantity = symbol.MinOrderQuantity,
                        MinNotionalValue = symbol.MinOrderValue,
                        QuantityDecimals = symbol.BaseAssetPrecision,
                        PriceDecimals = symbol.QuoteAssetPrecision,
                        ContractSize = 1,
                    };
                }).ToArray());
        }

        #endregion

        #region Futures Ticker client

        EndpointOptions<GetTickerRequest> IFuturesTickerRestClient.GetFuturesTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesTicker>> IFuturesTickerRestClient.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTicker>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedFuturesTicker>(Exchange, null, default);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return resultTicker.AsExchangeError<SharedFuturesTicker>(Exchange, new ServerError("Not found"));

            return resultTicker.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTicker(ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice, ticker.BaseVolume, null)
            {
                IndexPrice = ticker.IndexPrice,
                FundingRate = ticker.FundingRate,
                NextFundingTime = ticker.NextFundingRateTime
            });
        }

        EndpointOptions<GetTickersRequest> IFuturesTickerRestClient.GetFuturesTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<IEnumerable<SharedFuturesTicker>>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickersOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedFuturesTicker>>(Exchange, validationError);

            var resultTickers = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTickers)
                return resultTickers.AsExchangeResult<IEnumerable<SharedFuturesTicker>>(Exchange, null, default);

            return resultTickers.AsExchangeResult<IEnumerable<SharedFuturesTicker>>(Exchange, request.TradingMode == null ? SupportedTradingModes : new[] { request.TradingMode.Value }, resultTickers.Data.Select(x =>
            {
                return new SharedFuturesTicker(x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.BaseVolume, null)
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
            var validationError = ((ILeverageRestClient)this).GetLeverageOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Account.GetCollateralAccountSummaryAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedLeverage(result.Data.Leverage));
        }

        SetLeverageOptions ILeverageRestClient.SetLeverageOptions { get; } = new SetLeverageOptions();
        async Task<ExchangeWebResult<SharedLeverage>> ILeverageRestClient.SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = ((ILeverageRestClient)this).SetLeverageOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Account.SetAccountLeverageAsync((int)request.Leverage, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedLeverage(result.Data.Leverage));
        }
        #endregion

        #region Open Interest client

        EndpointOptions<GetOpenInterestRequest> IOpenInterestRestClient.GetOpenInterestOptions { get; } = new EndpointOptions<GetOpenInterestRequest>(true);
        async Task<ExchangeWebResult<SharedOpenInterest>> IOpenInterestRestClient.GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = ((IOpenInterestRestClient)this).GetOpenInterestOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOpenInterest>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedOpenInterest>(Exchange, null, default);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return resultTicker.AsExchangeError<SharedOpenInterest>(Exchange, new ServerError("Not found"));

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
        async Task<ExchangeWebResult<IEnumerable<SharedPositionHistory>>> IPositionHistoryRestClient.GetPositionHistoryAsync(GetPositionHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IPositionHistoryRestClient)this).GetPositionHistoryOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedPositionHistory>>(Exchange, validationError);

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
                return positions.AsExchangeResult<IEnumerable<SharedPositionHistory>>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (positions.Data.Count() == (request.Limit ?? 100))
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            return positions.AsExchangeResult<IEnumerable<SharedPositionHistory>>(Exchange, request.Symbol.TradingMode, positions.Data.Select(x => new SharedPositionHistory(
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

        IEnumerable<SharedOrderType> IFuturesOrderRestClient.FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market };
        IEnumerable<SharedTimeInForce> IFuturesOrderRestClient.FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport IFuturesOrderRestClient.FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        PlaceFuturesOrderOptions IFuturesOrderRestClient.PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions()
        {
            RequestNotes = "ReduceOnly is not supported"
        };
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).PlaceFuturesOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.Symbol.TradingMode,
                SupportedFuturesModes,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderTypes,
                ((IFuturesOrderRestClient)this).FuturesSupportedTimeInForce,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await CollateralTrading.PlaceOrderAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.OrderId.ToString()));
        }

        EndpointOptions<GetOrderRequest> IFuturesOrderRestClient.GetFuturesOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder>> IFuturesOrderRestClient.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, new ArgumentError("Invalid order id"));

            var openOrders = await Trading.GetOpenOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return openOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedFuturesOrder(
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
                    Quantity = openOrder.Quantity,
                    QuantityFilled = openOrder.QuantityFilled,
                    QuoteQuantity = null,
                    QuoteQuantityFilled = openOrder.QuoteQuantityFilled,
                    TimeInForce = ParseTimeInForce(openOrder),
                    Fee = openOrder.Fee
                });
            }
            else
            {
                var closeOrders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders)
                    return closeOrders.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

                if (!closeOrders.Data.Any())
                    return closeOrders.AsExchangeError<SharedFuturesOrder>(Exchange, new ServerError("Not found"));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status == ClosedOrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled;

                return closeOrders.AsExchangeResult(Exchange, TradingMode.Spot, new SharedFuturesOrder(
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
                        Quantity = closedOrder.Quantity,
                        QuantityFilled = closedOrder.QuantityFilled,
                        QuoteQuantity = null,
                        QuoteQuantityFilled = closedOrder.QuoteQuantityFilled,
                        TimeInForce = ParseTimeInForce(closedOrder),
                        Fee = closedOrder.Fee
                    });
            }            
        }

        EndpointOptions<GetOpenOrdersRequest> IFuturesOrderRestClient.GetOpenFuturesOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedFuturesOrder>>> IFuturesOrderRestClient.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetOpenFuturesOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedFuturesOrder>>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedFuturesOrder>>(Exchange, null, default);

            var data = orders.Data.Where(x => x.Symbol.EndsWith("_PERP"));

            return orders.AsExchangeResult<IEnumerable<SharedFuturesOrder>>(Exchange, TradingMode.Spot, data.Select(x => new SharedFuturesOrder(
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
                Quantity = x.Quantity,
                QuantityFilled = x.QuantityFilled,
                QuoteQuantity = null,
                QuoteQuantityFilled = x.QuoteQuantityFilled,
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> IFuturesOrderRestClient.GetClosedFuturesOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Descending, true, 100, true);
        async Task<ExchangeWebResult<IEnumerable<SharedFuturesOrder>>> IFuturesOrderRestClient.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetClosedFuturesOrdersOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedFuturesOrder>>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedFuturesOrder>>(Exchange, null, default);

            // Determine next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Where(x => x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedFuturesOrder(
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status == ClosedOrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                Quantity = x.Quantity,
                QuantityFilled = x.QuantityFilled,
                QuoteQuantity = null,
                QuoteQuantityFilled = x.QuoteQuantityFilled,
                TimeInForce = ParseTimeInForce(x),
                Fee = x.Fee
            }));

            return orders.AsExchangeResult<IEnumerable<SharedFuturesOrder>>(Exchange, TradingMode.Spot, data.OrderByDescending(x => x.CreateTime).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> IFuturesOrderRestClient.GetFuturesOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> IFuturesOrderRestClient.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, new ArgumentError("Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.Spot, orders.Data.Select(x => new SharedUserTrade(
                request.Symbol.GetSymbol(FormatSymbol),
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
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesUserTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            // Determine page token
            int? offset = null;
            if (pageToken is OffsetToken offsetToken)
                offset = offsetToken.Offset;

            // Get data
            var orders = await Trading.GetUserTradesAsync(request.Symbol.GetSymbol(FormatSymbol),
                offset: offset,
                limit: request.Limit ?? 100,
                ct: ct
                ).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            // Get next token
            OffsetToken? nextToken = null;
            if (orders.Data.Any())
                nextToken = new OffsetToken((offset ?? 0) + request.Limit ?? 100);

            var data = orders.Data.Select(y => new SharedUserTrade(
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

            return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.PerpetualLinear, data, nextToken);
        }

        EndpointOptions<CancelOrderRequest> IFuturesOrderRestClient.CancelFuturesOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).CancelFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(order.Data.OrderId.ToString()));
        }

        EndpointOptions<GetPositionsRequest> IFuturesOrderRestClient.GetPositionsOptions { get; } = new EndpointOptions<GetPositionsRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedPosition>>> IFuturesOrderRestClient.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetPositionsOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedPosition>>(Exchange, validationError);

            var result = await CollateralTrading.GetOpenPositionsAsync(symbol: request.Symbol?.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedPosition>>(Exchange, null, default);

            var data = result.Data;
            var resultTypes = request.Symbol == null && request.TradingMode == null ? SupportedTradingModes : request.Symbol != null ? new[] { request.Symbol.TradingMode } : new[] { request.TradingMode!.Value };
            return result.AsExchangeResult<IEnumerable<SharedPosition>>(Exchange, resultTypes, data.Select(x =>
            new SharedPosition(x.Symbol, Math.Abs(x.Quantity), x.UpdateTime)
            {
                UnrealizedPnl = x.Pnl,
                LiquidationPrice = x.LiquidationPrice == 0 ? null : x.LiquidationPrice,
                AverageOpenPrice = x.BasePrice,
                PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,                
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
            var validationError = ((IFuturesOrderRestClient)this).ClosePositionOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedFuturesModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
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
            var validationError = ((IFeeRestClient)this).GetFeeOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFee>(Exchange, validationError);

            // Get data
            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFee>(Exchange, null, default);

            var symbol = result.Data.SingleOrDefault(x => x.Name == request.Symbol.GetSymbol(FormatSymbol));
            if (symbol == null)
                return result.AsExchangeError<SharedFee>(Exchange, new ServerError("Not found"));

            // Return
            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedFee(symbol.MakerFee, symbol.TakerFee));
        }
        #endregion
    }
}

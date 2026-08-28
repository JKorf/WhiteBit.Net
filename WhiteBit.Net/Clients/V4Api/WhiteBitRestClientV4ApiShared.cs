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
    internal class WhiteBitRestClientV4SharedApi :
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

        #region Spot Symbol client
        public SharedSymbolCatalog? SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchange, _topicSpotId, _api.EnvironmentName, null);
        public GetSpotSymbolsOptions GetSpotSymbolsOptions { get; } = new GetSpotSymbolsOptions(_exchange, false);

        public async Task<HttpResult<SharedSpotSymbol[]>> GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(result);

            var resultData =
                 result.Data
                 .Where(x => x.SymbolType == Enums.SymbolType.Spot)
                 .Select(x => ParseSpotSymbol(x))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, _api.EnvironmentName, null, resultData);
            return HttpResult.Ok(result, SharedUtils.ApplySymbolFilter(resultData, request));
        }

        private SharedSpotSymbol ParseSpotSymbol(WhiteBitSymbol s)
        {
            var result = new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name, s.TradingEnabled)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                MinNotionalValue = s.MinOrderValue,
                QuantityDecimals = s.BaseAssetPrecision,
                PriceDecimals = s.QuoteAssetPrecision,
                DisplayName = s.Name,
                BaseAssetType = SharedAssetType.Crypto,
                MakerFeePercentage = s.MakerFee,
                TakerFeePercentage = s.TakerFee,
                PriceStep = s.TickSize,
                QuantityStep = s.StepSize
            };

            if (LibraryHelpers.IsStableCoin(result.BaseAsset))
            {
                result.BaseAssetSubType = SharedAssetSubType.StableCoin;
            }

            if (_exchangeFiats.Contains(s.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.Fiat;
            }
            else
            {
                result.QuoteAssetType = SharedAssetType.Crypto;
                if (LibraryHelpers.IsStableCoin(result.QuoteAsset))
                    result.QuoteAssetSubType = SharedAssetSubType.StableCoin;
            }

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicSpotId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbolName));
        }
        #endregion

        #region Ticker client

        Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
            => GetAllSpotTickersAsync(request, ct);
        GetAllSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions => GetAllSpotTickersOptions;
        public GetSpotTickerOptions GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchange);
        public async Task<HttpResult<SharedSpotTicker>> GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetSpotTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker>(Exchange, validationError);

            var result = await _api.ExchangeData.GetTickersAsync(ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker>(result);

            var ticker = result.Data.SingleOrDefault(x => x.Symbol == request.Symbol!.GetSymbol(FormatSymbol));
            if (ticker == null)
                return HttpResult.Fail<SharedSpotTicker>(result, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return HttpResult.Ok(result, new SharedSpotTicker(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, ticker.Symbol), 
                ticker.Symbol,
                ticker.LastPrice,
                null, 
                null, 
                new SharedOrderQuantity(ticker.BaseVolume, ticker.QuoteVolume), 
                ticker.ChangePercentage)
            {
            });
        }

        public GetAllSpotTickersOptions GetAllSpotTickersOptions { get; } = new GetAllSpotTickersOptions(_exchange);
        public async Task<HttpResult<SharedSpotTicker[]>> GetAllSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker[]>(result);

            var data = result.Data.Where(x => !x.Symbol.EndsWith("_PERP"));
            return HttpResult.Ok(result, data.Select(x => 
            new SharedSpotTicker(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol), 
                x.Symbol, 
                x.LastPrice,
                null,
                null,
                new SharedOrderQuantity(x.BaseVolume, x.QuoteVolume),
                x.ChangePercentage)
            {
            }).ToArray());
        }

        #endregion

        #region Book Ticker client

        public GetBookTickerOptions GetBookTickerOptions { get; } = new GetBookTickerOptions(_exchange, false);
        public async Task<HttpResult<SharedBookTicker>> GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = GetBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBookTicker>(Exchange, validationError);

            var resultTicker = await _api.ExchangeData.GetOrderBookAsync(request.Symbol!.GetSymbol(FormatSymbol), 1, ct: ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedBookTicker>(resultTicker);

            return HttpResult.Ok(resultTicker, new SharedBookTicker(
                ExchangeSymbolCache.ParseSymbol(request.Symbol.TradingMode == TradingMode.Spot ? _topicSpotId : _topicFuturesId, _api.EnvironmentName, null, resultTicker.Data.Symbol),
                resultTicker.Data.Symbol,
                resultTicker.Data.Asks[0].Price,
                new SharedOrderQuantity(resultTicker.Data.Asks[0].Quantity),
                resultTicker.Data.Bids[0].Price,
                new SharedOrderQuantity(resultTicker.Data.Bids[0].Quantity)));
        }

        #endregion

        #region Recent Trades client
        public GetRecentTradesOptions GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchange, 100, false);

        public async Task<HttpResult<SharedTrade[]>> GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            // Get data
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.ExchangeData.GetRecentTradesAsync(
                symbol,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            IEnumerable<WhiteBitTrade> data = result.Data;
            if (request.Limit != null)
                data = data.Take(request.Limit.Value);

            // Return
            return HttpResult.Ok(result, data.Select(x =>
                new SharedTrade(request.Symbol, symbol, new SharedOrderQuantity(x.BaseVolume, x.QuoteVolume), x.Price, x.Timestamp)
                {
                    Side = x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
                }).ToArray());
        }
        #endregion

        #region Order Book client
        public GetOrderBookOptions GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchange, 1, 100, false);
        public async Task<HttpResult<SharedOrderBook>> GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await _api.ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            return HttpResult.Ok(result, new SharedOrderBook(SharedQuantityType.BaseAsset, null, result.Data.Asks, result.Data.Bids));
        }

        #endregion

        #region Balance Client
        public GetBalancesOptions GetBalancesOptions { get; } = new GetBalancesOptions(_exchange, AccountTypeFilter.Funding, AccountTypeFilter.Spot, AccountTypeFilter.Futures);

        public async Task<HttpResult<SharedBalance[]>> GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            if (request.AccountType == null || request.AccountType == SharedAccountType.Spot)
            {
                var result = await _api.Account.GetSpotBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result.Success)
                    return HttpResult.Fail<SharedBalance[]>(result);

                return HttpResult.Ok(result, result.Data.Select(x => 
                    new SharedBalance(TradingMode.Spot, x.Asset, x.Available, x.Available + x.Frozen)).ToArray());
            }
            else if(request.AccountType == SharedAccountType.Funding)
            {
                var result = await _api.Account.GetMainBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result.Success)
                    return HttpResult.Fail<SharedBalance[]>(result);

                return HttpResult.Ok(result, result.Data.Select(x => 
                    new SharedBalance([], x.Asset, x.MainBalance, x.MainBalance)).ToArray());
            }
            else
            {
                var result = await _api.Account.GetCollateralBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result.Success)
                    return HttpResult.Fail<SharedBalance[]>(result);

                return HttpResult.Ok(result, result.Data.Select(x =>
                    new SharedBalance([TradingMode.PerpetualLinear], x.Key, x.Value, x.Value)).ToArray());
            }
        }

        #endregion

        #region Asset client
        public GetAllAssetsOptions GetAllAssetsOptions { get; } = new GetAllAssetsOptions(_exchange, false);

        public async Task<HttpResult<SharedAsset[]>> GetAllAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = GetAllAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            var assets = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets.Success)
                return HttpResult.Fail<SharedAsset[]>(assets);

            return HttpResult.Ok(assets, assets.Data.Select(x =>
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

        public GetAssetOptions GetAssetOptions { get; } = new GetAssetOptions(_exchange, false);
        public async Task<HttpResult<SharedAsset>> GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            var assets = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets.Success)
                return HttpResult.Fail<SharedAsset>(assets);

            var asset = assets.Data.SingleOrDefault(x => x.Asset == request.Asset);
            if (asset == null)
                return HttpResult.Fail<SharedAsset>(assets, new ServerError(new ErrorInfo(ErrorType.UnknownAsset, "Asset not found")));

            var networks = asset.Networks.Withdraws.Intersect(asset.Networks.Deposits);
            return HttpResult.Ok(assets, new SharedAsset(asset.Asset)
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

        public GetDepositAddressesOptions GetDepositAddressesOptions { get; } = new GetDepositAddressesOptions(_exchange, true);
        public async Task<HttpResult<SharedDepositAddress[]>> GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = GetDepositAddressesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDepositAddress[]>(Exchange, validationError);

            var depositAddresses = await _api.Account.GetDepositAddressAsync(request.Asset, request.Network, ct: ct).ConfigureAwait(false);
            if (!depositAddresses.Success)
                return HttpResult.Fail<SharedDepositAddress[]>(depositAddresses);

            return HttpResult.Ok(depositAddresses, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Account.Address)
            {
                Network = request.Network
            }
            });
        }

        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetDepositHistoryAsync(request, pageRequest, ct);
        GetDepositHistoryOptions IDepositRestClient.GetDepositsOptions => GetDepositHistoryOptions;

        public GetDepositHistoryOptions GetDepositHistoryOptions { get; } = new GetDepositHistoryOptions(_exchange, false, true, false, 100);
        public async Task<HttpResult<SharedDeposit[]>> GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetDepositHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Account.GetDepositWithdrawalHistoryAsync(
                Enums.TransactionType.Deposit,
                request.Asset,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedDeposit[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Records.Length,
                result.Data.Records.Select(x => x.CreateTime),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Records, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                       .Select(x =>
                            new SharedDeposit(
                                x.Asset,
                                x.Quantity,
                                x.TransactionStatus == Enums.TransactionStatus.Success,
                                x.CreateTime,
                                ParseTransferStatus(x.TransactionStatus))
                            {
                                Confirmations = x.Confirmations?.Actual,
                                Network = x.Network,
                                TransactionId = x.TransactionId,
                                Tag = x.Memo,
                                Id = x.UniqueId
                            })
                       .ToArray(), nextPageRequest);
        }

        private SharedTransferStatus ParseTransferStatus(TransactionStatus? transactionStatus)
        {
            if (transactionStatus == TransactionStatus.Success)
                return SharedTransferStatus.Completed;

            if (transactionStatus == TransactionStatus.UnconfirmedByUser || transactionStatus == TransactionStatus.Canceled)
                return SharedTransferStatus.Failed;

            if (transactionStatus == TransactionStatus.AwaitingVerification
                || transactionStatus == TransactionStatus.ConfirmationInProgress
                || transactionStatus == TransactionStatus.Pending
                || transactionStatus == TransactionStatus.Uncredited)
            {
                return SharedTransferStatus.InProgress;
            }

            return SharedTransferStatus.Unknown;
        }

        #endregion

        #region Withdrawal client

        Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetWithdrawalHistoryAsync(request, pageRequest, ct);
        GetWithdrawalHistoryOptions IWithdrawalRestClient.GetWithdrawalsOptions => GetWithdrawalHistoryOptions;

        public GetWithdrawalHistoryOptions GetWithdrawalHistoryOptions { get; } = new GetWithdrawalHistoryOptions(_exchange, false, true, false, 100);
        public async Task<HttpResult<SharedWithdrawal[]>> GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetWithdrawalHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Account.GetDepositWithdrawalHistoryAsync(
                Enums.TransactionType.Withdrawal,
                request.Asset,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedWithdrawal[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Records.Length,
                result.Data.Records.Select(x => x.CreateTime),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Records, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                       .Select(x => 
                           new SharedWithdrawal(
                               x.Asset,
                               x.Address,
                               x.Quantity,
                               x.TransactionStatus == Enums.TransactionStatus.Success,
                               x.CreateTime,
                               GetWithdrawalStatus(x))
                           {
                               Confirmations = x.Confirmations?.Actual,
                               Network = x.Network,
                               Tag = x.Memo,
                               TransactionId = x.TransactionId,
                               Fee = x.Fee,
                               Id = x.UniqueId
                           })
                    .ToArray(), nextPageRequest);
        }

        private SharedTransferStatus GetWithdrawalStatus(WhiteBitDepositWithdrawal x)
        {
            if (x.TransactionStatus == TransactionStatus.Canceled || x.TransactionStatus == TransactionStatus.UnconfirmedByUser)
                return SharedTransferStatus.Failed;

            if (x.TransactionStatus == TransactionStatus.Success || x.TransactionStatus == TransactionStatus.PartialSuccess)
                return SharedTransferStatus.Completed;

            if (x.TransactionStatus == TransactionStatus.AwaitingVerification
                || x.TransactionStatus == TransactionStatus.ConfirmationInProgress
                || x.TransactionStatus == TransactionStatus.Frozen
                || x.TransactionStatus == TransactionStatus.Pending
                || x.TransactionStatus == TransactionStatus.Uncredited)
            {
                return SharedTransferStatus.InProgress;
            }

            return SharedTransferStatus.Unknown;
        }
        #endregion

        #region Withdraw client

        public WithdrawOptions WithdrawOptions { get; } = new WithdrawOptions(_exchange);
        public async Task<HttpResult<SharedId>> WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = WithdrawOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var id = Guid.NewGuid().ToString();
            // Get data
            var withdrawal = await _api.Account.WithdrawAsync(
                request.Asset,
                request.Quantity,
                request.Address,
                id,
                true,
                network: request.Network,
                memo: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal.Success)
                return HttpResult.Fail<SharedId>(withdrawal);

            return HttpResult.Ok(withdrawal, new SharedId(id));
        }

        #endregion

        #region Spot Order Client

        public SharedFeeDeductionType SpotFeeDeductionType => SharedFeeDeductionType.AddToCost;
        public SharedFeeAssetType SpotFeeAssetType => SharedFeeAssetType.QuoteAsset;
        public SharedOrderType[] SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        public SharedQuantitySupport SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.QuoteAsset,
                SharedQuantityType.BaseAsset);

        public string GenerateClientOrderId() => ExchangeHelpers.RandomString(32);

        public PlaceSpotOrderOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchange);
        public async Task<HttpResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.Trading.PlaceSpotOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit || request.OrderType == SharedOrderType.LimitMaker ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset,
                quoteQuantity: request.Quantity?.QuantityInQuoteAsset,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        public GetSpotOrderOptions GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await _api.Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotOrder>(openOrders);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return HttpResult.Ok(openOrders, new SharedSpotOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, openOrder.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                    Fee = openOrder.Fee,
                    FeeAsset = openOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TriggerPrice = openOrder.TriggerPrice,
                    IsTriggerOrder = openOrder.TriggerPrice > 0
                });
            }
            else
            {
                var closeOrders = await _api.Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders.Success)
                    return HttpResult.Fail<SharedSpotOrder>(closeOrders);

                if (!closeOrders.Data.Any())
                    return HttpResult.Fail<SharedSpotOrder>(closeOrders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status == OrderStatus.Canceled ? SharedOrderStatus.Canceled : SharedOrderStatus.Filled;

                return HttpResult.Ok(closeOrders, new SharedSpotOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, closedOrder.Symbol),
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
#pragma warning disable CS0618 // Type or member is obsolete
                    Fee = closedOrder.Fee,
                    FeeAsset = closedOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TriggerPrice = closedOrder.TriggerPrice,
                    IsTriggerOrder = closedOrder.TriggerPrice > 0
                });
            }
        }

        public GetOpenSpotOrdersOptions GetOpenSpotOrdersOptions { get; } = new GetOpenSpotOrdersOptions(_exchange, true);
        public async Task<HttpResult<SharedSpotOrder[]>> GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);

            var allOpenOrders = new List<WhiteBitOrder>();
            int offset = 0;
            HttpResult<WhiteBitOrder[]> orders;
            while (true)
            {
                orders = await _api.Trading.GetOpenOrdersAsync(symbol, limit: 100, offset: offset, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                    return HttpResult.Fail<SharedSpotOrder[]>(orders);

                allOpenOrders.AddRange(orders.Data);
                if (orders.Data.Length == 100)
                    offset += 100;
                else
                    break;
            }

            var data = allOpenOrders.Where(x => !x.Symbol.EndsWith("_PERP"));

            return HttpResult.Ok(orders, data.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }).ToArray());
        }

        public GetSpotClosedOrdersOptions GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchange, true, false, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedSpotOrder[]>> GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, maxPeriod: TimeSpan.FromDays(31));

            // Get data
            var result = await _api.Trading.GetClosedOrdersAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Values.Count,
                result.Data.Values.SelectMany(x => x.Select(x => x.CreateTime)),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams,
                TimeSpan.FromDays(31),
                TimeSpan.FromDays(180));

            var data = result.Data.Where(x => !x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0
            }));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.CreateTime!.Value, request.StartTime, request.EndTime, direction).ToArray(), nextPageRequest);
        }

        public GetSpotOrderTradesOptions GetSpotOrderTradesOptions { get; } = new GetSpotOrderTradesOptions(_exchange, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.OrderSide == null ? (SharedOrderSide?)null : x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.Time)
            {
                ClientOrderId = x.ClientOrderId,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetSpotUserTradeHistoryAsync(request, pageRequest, ct);
        GetSpotUserTradeHistoryOptions ISpotOrderRestClient.GetSpotUserTradesOptions => GetSpotUserTradeHistoryOptions;

        public GetSpotUserTradeHistoryOptions GetSpotUserTradeHistoryOptions { get; } = new GetSpotUserTradeHistoryOptions(_exchange, false, true, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetSpotUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Length,
                result.Data.Select(x => x.Time),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams,
                maxAge: TimeSpan.FromDays(180));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Time, request.StartTime, request.EndTime, direction)
                       .Select(y => new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, y.Symbol),
                            y.Symbol,
                            y.OrderId.ToString(),
                            y.Id.ToString(),
                            y.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            new SharedOrderQuantity(y.Quantity),
                            y.Price,
                            y.Time)
                        {
                            ClientOrderId = y.ClientOrderId,
                            Fee = y.Fee,
                            FeeAsset = y.FeeAsset,
                            Role = y.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
                        }).ToArray(), nextPageRequest);
        }

        public CancelSpotOrderOptions CancelSpotOrderOptions { get; } = new CancelSpotOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.OrderId.ToString()));
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

        public SharedSymbolCatalog? FuturesSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchange, _topicFuturesId, _api.EnvironmentName, null);
        public GetFuturesSymbolsOptions GetFuturesSymbolsOptions { get; } = new GetFuturesSymbolsOptions(_exchange, false);
        public async Task<HttpResult<SharedFuturesSymbol[]>> GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesSymbol[]>(Exchange, validationError);

            var symbolsTask = _api.ExchangeData.GetSymbolsAsync(ct);
            var futuresSymbolsTask = _api.ExchangeData.GetFuturesSymbolsAsync(ct);
            await Task.WhenAll(symbolsTask, futuresSymbolsTask).ConfigureAwait(false);

            var symbols = symbolsTask.Result;
            var futuresSymbols = futuresSymbolsTask.Result;
            if (!symbols.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(symbols);
            if (!futuresSymbols.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(futuresSymbols);

            var resultData =
                 futuresSymbols.Data
                 .Select(x => ParseFuturesSymbol(x, symbols.Data))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicFuturesId, _api.EnvironmentName, null, resultData);
            return HttpResult.Ok(symbols, SharedUtils.ApplySymbolFilter(resultData, request));
        }

        private SharedFuturesSymbol ParseFuturesSymbol(WhiteBitFuturesSymbol s, WhiteBitSymbol[] symbols)
        {
            var symbol = symbols.SingleOrDefault(x => x.Name == s.Symbol);
            var result = new SharedFuturesSymbol(s.ProductType == ProductType.Perpetual ? TradingMode.PerpetualLinear : TradingMode.DeliveryLinear, s.BaseAsset, s.QuoteAsset, s.Symbol, true)
            {
                MinTradeQuantity = symbol?.MinOrderQuantity,
                MinNotionalValue = symbol?.MinOrderValue,
                QuantityDecimals = symbol?.BaseAssetPrecision,
                PriceDecimals = symbol?.QuoteAssetPrecision,
                ContractSize = 1,
                DisplayName = s.Symbol,
                QuoteAssetType = SharedAssetType.Crypto,
                QuoteAssetSubType = SharedAssetSubType.StableCoin,
                MakerFeePercentage = symbol?.MakerFee,
                TakerFeePercentage = symbol?.TakerFee,
                UpperFundingCap = s.FundingCap,
                LowerFundingCap = s.FundingFloor,
                MaxShortLeverage = s.MaxLeverage,
                MaxLongLeverage = s.MaxLeverage,
                PriceStep = symbol?.TickSize,
                QuantityStep = symbol?.StepSize
            };

            if (symbol?.IsTradFiFutures == true)
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                if (LibraryHelpers.IsCommodity(result.BaseAsset))
                    result.BaseAssetSubType = SharedAssetSubType.Commodity;
                else
                    result.BaseAssetSubType = SharedAssetSubType.Equity;
            }
            else if (symbol != null)
            {
                result.BaseAssetType = SharedAssetType.Crypto;
            }

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetFuturesSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicFuturesId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode == TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Spot symbols not allowed");

            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, _api.EnvironmentName, null, symbolName));
        }

        #endregion

        #region Futures Ticker client

        public GetFuturesTickerOptions GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchange);
        public async Task<HttpResult<SharedFuturesTicker>> GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var resultTicker = await _api.ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedFuturesTicker>(resultTicker);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return HttpResult.Fail<SharedFuturesTicker>(resultTicker, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return HttpResult.Ok(resultTicker,
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, ticker.Symbol),
                    ticker.Symbol,
                    ticker.LastPrice,
                    ticker.HighPrice,
                    ticker.LowPrice,
                    new SharedOrderQuantity(ticker.BaseVolume, ticker.QuoteVolume), 
                    null)
            {
                IndexPrice = ticker.IndexPrice,
                FundingRate = ticker.FundingRate,
                NextFundingTime = ticker.NextFundingRateTime
            });
        }

        Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
            => GetAllFuturesTickersAsync(request, ct);
        GetAllFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions => GetAllFuturesTickersOptions;
        public GetAllFuturesTickersOptions GetAllFuturesTickersOptions { get; } = new GetAllFuturesTickersOptions(_exchange);
        public async Task<HttpResult<SharedFuturesTicker[]>> GetAllFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllFuturesTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var resultTickers = await _api.ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTickers.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(resultTickers);

            return HttpResult.Ok(resultTickers, resultTickers.Data.Select(x =>
            {
                return new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                    x.Symbol, 
                    x.LastPrice,
                    x.HighPrice,
                    x.LowPrice,
                    new SharedOrderQuantity(x.BaseVolume, x.QuoteVolume),
                    null)
                {
                    IndexPrice = x.IndexPrice,
                    FundingRate = x.FundingRate,
                    NextFundingTime = x.NextFundingRateTime
                };
            }).ToArray());
        }

        #endregion

        #region Leverage client
        public SharedLeverageSettingMode LeverageSettingType => SharedLeverageSettingMode.PerAccount;

        public GetLeverageOptions GetLeverageOptions { get; } = new GetLeverageOptions(_exchange, true);
        public async Task<HttpResult<SharedLeverage>> GetLeverageAsync(GetLeverageRequest request, CancellationToken ct)
        {
            var validationError = GetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await _api.Account.GetCollateralAccountSummaryAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedLeverage>(result);

            return HttpResult.Ok(result, new SharedLeverage(result.Data.Leverage));
        }

        public SetLeverageOptions SetLeverageOptions { get; } = new SetLeverageOptions(_exchange);
        public async Task<HttpResult<SharedLeverage>> SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = SetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await _api.Account.SetAccountLeverageAsync((int)request.Leverage, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedLeverage>(result);

            return HttpResult.Ok(result, new SharedLeverage(result.Data.Leverage));
        }
        #endregion

        #region Open Interest client

        public GetOpenInterestOptions GetOpenInterestOptions { get; } = new GetOpenInterestOptions(_exchange, false);
        public async Task<HttpResult<SharedOpenInterest>> GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = GetOpenInterestOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOpenInterest>(Exchange, validationError);

            var resultTicker = await _api.ExchangeData.GetFuturesSymbolsAsync(ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedOpenInterest>(resultTicker);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var ticker = resultTicker.Data.SingleOrDefault(x => x.Symbol == symbol);
            if (ticker == null)
                return HttpResult.Fail<SharedOpenInterest>(resultTicker, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return HttpResult.Ok(resultTicker, new SharedOpenInterest(new SharedOrderQuantity(ticker.OpenInterest)));
        }

        #endregion

        #region Position History client

        public GetPositionHistoryOptions GetPositionHistoryOptions { get; } = new GetPositionHistoryOptions(_exchange, false, true, true, 100)
        {
            RequiredRequestParameters = [
                RequestParameter<GetPositionHistoryRequest>.Required(x => x.Symbol, "The symbol to get position history for", new SharedSymbol(TradingMode.PerpetualLinear, "ETH", "USDT"))
                ]
        };
        public async Task<HttpResult<SharedPositionHistory[]>> GetPositionHistoryAsync(GetPositionHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetPositionHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPositionHistory[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.CollateralTrading.GetPositionHistoryAsync(
                symbol: request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPositionHistory[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Length,
                result.Data.Select(x => x.OpenTime),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.OpenTime, request.StartTime, request.EndTime, direction)
                       .Select(x => 
                           new SharedPositionHistory(
                                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                                x.Symbol,
                                x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                                x.BasePrice,
                                x.OrderDetail.Price,
                                new SharedOrderQuantity(x.OrderDetail.TradeQuantity),
                                x.OrderDetail.RealizedPnl ?? 0,
                                x.OpenTime)
                            {
                                PositionId = x.PositionId.ToString()
                            }).ToArray(), nextPageRequest);
        }
        #endregion

        #region Futures Order Client

        public SharedFeeDeductionType FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        public SharedFeeAssetType FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        public SharedOrderType[] FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.LimitMaker, SharedOrderType.Market };
        public SharedTimeInForce[] FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        public SharedQuantitySupport FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        public PlaceFuturesOrderOptions PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                (request.OrderType == SharedOrderType.Limit || request.OrderType == SharedOrderType.LimitMaker) ? Enums.NewOrderType.Limit : Enums.NewOrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts,
                price: request.Price,
                postOnly: request.OrderType == SharedOrderType.LimitMaker ? true : null,
                immediateOrCancel: request.TimeInForce == SharedTimeInForce.ImmediateOrCancel ? true : null,
                clientOrderId: request.ClientOrderId,
                takeProfitPrice: request.TakeProfitPrice,
                stopLossPrice: request.StopLossPrice,
                positionSide: request.PositionSide.HasValue ? request.PositionSide.Value.ToPositionSide() : null,
                reduceOnly: request.ReduceOnly,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        public GetFuturesOrderOptions GetFuturesOrderOptions { get; } = new GetFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedFuturesOrder>> GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await _api.Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, openOrder.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                    Fee = openOrder.Fee,
                    FeeAsset = openOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TakeProfitPrice = openOrder.OtoData?.TakeProfit,
                    StopLossPrice = openOrder.OtoData?.StopLoss,
                    TriggerPrice = openOrder.TriggerPrice,
                    IsTriggerOrder = openOrder.TriggerPrice > 0,
                    UpdateTime = openOrder.UpdateTime
                });
            }
            else
            {
                var closeOrders = await _api.Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closeOrders);

                if (!closeOrders.Data.Any())
                    return HttpResult.Fail<SharedFuturesOrder>(closeOrders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                var status = closedOrder.Status is OrderStatus.Canceled or OrderStatus.AutoCanceledUserMargin 
                    ? SharedOrderStatus.Canceled 
                    : SharedOrderStatus.Filled;

                return HttpResult.Ok(closeOrders, new SharedFuturesOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, closedOrder.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                        Fee = closedOrder.Fee,
                        FeeAsset = closedOrder.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                    TakeProfitPrice = closedOrder.OtoData?.TakeProfit,
                        StopLossPrice = closedOrder.OtoData?.StopLoss,
                        TriggerPrice = closedOrder.TriggerPrice,
                        IsTriggerOrder = closedOrder.TriggerPrice > 0,
                        UpdateTime = closedOrder.FillTime ?? closedOrder.UpdateTime
                });
            }            
        }

        public GetOpenFuturesOrdersOptions GetOpenFuturesOrdersOptions { get; } = new GetOpenFuturesOrdersOptions(_exchange, true);
        public async Task<HttpResult<SharedFuturesOrder[]>> GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var allOpenOrders = new List<WhiteBitOrder>();
            int offset = 0;
            HttpResult<WhiteBitOrder[]> orders;
            while (true)
            {
                orders = await _api.Trading.GetOpenOrdersAsync(symbol, limit: 100, offset: offset, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                    return HttpResult.Fail<SharedFuturesOrder[]>(orders);

                allOpenOrders.AddRange(orders.Data);
                if (orders.Data.Length == 100)
                    offset += 100;
                else
                    break;
            }

            var data = allOpenOrders.Where(x => x.Symbol.EndsWith("_PERP"));

            return HttpResult.Ok<SharedFuturesOrder[]>(orders, [.. data.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
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
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0,
                UpdateTime = x.UpdateTime
            })]);
        }

        public GetFuturesClosedOrdersOptions GetClosedFuturesOrdersOptions { get; } = new GetFuturesClosedOrdersOptions(_exchange, false, true, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedFuturesOrder[]>> GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, maxPeriod: TimeSpan.FromDays(31));

            // Get data
            var result = await _api.Trading.GetClosedOrdersAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
               () => Pagination.NextPageFromOffset(pageParams, limit),
               result.Data.Values.Count,
               result.Data.Values.SelectMany(x => x.Select(x => x.CreateTime)),
               request.StartTime,
               request.EndTime ?? DateTime.UtcNow,
               pageParams,
               TimeSpan.FromDays(31),
               TimeSpan.FromDays(180));

            var data = result.Data.Where(x => x.Key.EndsWith("_PERP")).SelectMany(xk => xk.Value.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, xk.Key), 
                xk.Key,
                x.OrderId.ToString(),
                ParseOrderType(x.OrderType, x.PostOnly),
                x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Status is OrderStatus.Canceled or OrderStatus.AutoCanceledUserMargin
                    ? SharedOrderStatus.Canceled
                    : SharedOrderStatus.Filled,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId == string.Empty ? null : x.ClientOrderId,
                AveragePrice = x.QuantityFilled != 0 ? x.QuoteQuantityFilled / x.QuantityFilled : null,
                OrderPrice = x.Price == 0 ? null : x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled, x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x),
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
#pragma warning restore CS0618 // Type or member is obsolete
                TakeProfitPrice = x.OtoData?.TakeProfit,
                StopLossPrice = x.OtoData?.StopLoss,
                TriggerPrice = x.TriggerPrice,
                IsTriggerOrder = x.TriggerPrice > 0,
                UpdateTime = x.FillTime ?? x.UpdateTime
            }));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.CreateTime!.Value, request.StartTime, request.EndTime, direction).ToArray(), nextPageRequest);
        }

        public GetFuturesOrderTradesOptions GetFuturesOrderTradesOptions { get; } = new GetFuturesOrderTradesOptions(_exchange, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetOrderTradesAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                request.Symbol!.GetSymbol(FormatSymbol),
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.OrderSide == null ? (SharedOrderSide?)null : x.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.Time)
            {
                ClientOrderId = x.ClientOrderId,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetFuturesUserTradeHistoryAsync(request, pageRequest, ct);
        GetFuturesUserTradeHistoryOptions IFuturesOrderRestClient.GetFuturesUserTradesOptions => GetFuturesUserTradeHistoryOptions;

        public GetFuturesUserTradeHistoryOptions GetFuturesUserTradeHistoryOptions { get; } = new GetFuturesUserTradeHistoryOptions(_exchange, false, true, true, 100)
        {
            MaxAge = TimeSpan.FromDays(180)
        };
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetFuturesUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromOffset(pageParams, limit),
                result.Data.Length,
                result.Data.Select(x => x.Time),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams,
                maxAge: TimeSpan.FromDays(180));

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Time, request.StartTime, request.EndTime, direction)
                       .Select(y => 
                           new SharedUserTrade(
                                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, y.Symbol), 
                                y.Symbol,
                                y.OrderId.ToString(),
                                y.Id.ToString(),
                                y.OrderSide == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                                new SharedOrderQuantity(y.Quantity),
                                y.Price,
                                y.Time)
                            {
                                ClientOrderId = y.ClientOrderId,
                                Fee = y.Fee,
                                FeeAsset = y.FeeAsset,
                                Role = y.TradeRole == TradeRole.Maker ? SharedRole.Maker : SharedRole.Taker
                            })
                       .ToArray(), nextPageRequest);
        }

        public CancelFuturesOrderOptions CancelFuturesOrderOptions { get; } = new CancelFuturesOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.OrderId.ToString()));
        }

        public GetPositionsOptions GetPositionsOptions { get; } = new GetPositionsOptions(_exchange, true);
        public async Task<HttpResult<SharedPosition[]>> GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = GetPositionsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPosition[]>(Exchange, validationError);

            var result = await _api.CollateralTrading.GetOpenPositionsAsync(symbol: request.Symbol?.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPosition[]>(result);

            var data = result.Data;
            var resultTypes = request.Symbol == null && request.TradingMode == null ? SupportedTradingModes : request.Symbol != null ? new[] { request.Symbol.TradingMode } : new[] { request.TradingMode!.Value };
            return HttpResult.Ok(result, data.Select(x =>
            new SharedPosition(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                x.Symbol,
                new SharedOrderQuantity(Math.Abs(x.Quantity)),
                x.UpdateTime)
            {
                UnrealizedPnl = x.Pnl,
                LiquidationPrice = x.LiquidationPrice == 0 ? null : x.LiquidationPrice,
                AverageOpenPrice = x.BasePrice,
                PositionMode = SharedPositionMode.OneWay,
                PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short, 
                TakeProfitPrice = x.TpSl?.TakeProfitPrice,
                StopLossPrice = x.TpSl?.StopLossPrice
            }).ToArray());
        }

        public ClosePositionOptions ClosePositionOptions { get; } = new ClosePositionOptions(_exchange, true)
        {
            RequiredRequestParameters = [
                RequestParameter<ClosePositionRequest>.Required(x => x.PositionSide, "Current side of the position to close", SharedPositionSide.Long),
                RequestParameter<ClosePositionRequest>.Required(x => x.Quantity,"Quantity of the position is required", 0.1m)
                ]
        };
        public async Task<HttpResult<SharedId>> ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = ClosePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.CollateralTrading.PlaceOrderAsync(
                symbol,
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                NewOrderType.Market,
                request.Quantity,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        #endregion

        #region Fee Client
        public GetFeeOptions GetFeeOptions { get; } = new GetFeeOptions(_exchange, false);

        public async Task<HttpResult<SharedFee>> GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await _api.ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            var symbol = result.Data.SingleOrDefault(x => x.Name == request.Symbol!.GetSymbol(FormatSymbol));
            if (symbol == null)
                return HttpResult.Fail<SharedFee>(result, new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            // Return
            return HttpResult.Ok(result, new SharedFee(symbol.MakerFee, symbol.TakerFee));
        }
        #endregion

        #region Spot Trigger Order Client

        public PlaceSpotTriggerOrderOptions PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchange, true)
        {
        };

        public async Task<HttpResult<SharedId>> PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.Trading.PlaceSpotOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.OrderSide == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                request.OrderPrice == null ? NewOrderType.StopMarket : NewOrderType.StopLimit,
                quantity: request.Quantity.QuantityInBaseAsset,
                quoteQuantity: request.Quantity.QuantityInQuoteAsset,
                triggerPrice: request.TriggerPrice,
                price: request.OrderPrice,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        public GetSpotTriggerOrderOptions GetSpotTriggerOrderOptions { get; } = new GetSpotTriggerOrderOptions(_exchange, true)
        {
        };
        public async Task<HttpResult<SharedSpotTriggerOrder>> GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await _api.Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(openOrders);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return HttpResult.Ok(openOrders, new SharedSpotTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, openOrder.Symbol),
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
                    FeeAsset = openOrder.FeeAsset,
                    ClientOrderId = openOrder.ClientOrderId
                });
            }
            else
            {
                var closeOrders = await _api.Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders.Success)
                    return HttpResult.Fail<SharedSpotTriggerOrder>(closeOrders);

                if (!closeOrders.Data.Any())
                    return HttpResult.Fail<SharedSpotTriggerOrder>(closeOrders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                return HttpResult.Ok(closeOrders, new SharedSpotTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, closedOrder.Symbol),
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
                    FeeAsset = closedOrder.FeeAsset,
                    ClientOrderId = closedOrder.ClientOrderId
                });
            }
        }

        public CancelSpotTriggerOrderOptions CancelSpotTriggerOrderOptions { get; } = new CancelSpotTriggerOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));
        }

        #endregion

        #region Futures Trigger Order Client
        public PlaceFuturesTriggerOrderOptions PlaceFuturesTriggerOrderOptions { get; } = new PlaceFuturesTriggerOrderOptions(_exchange, false)
        {
        };

        public async Task<HttpResult<SharedId>> PlaceFuturesTriggerOrderAsync(PlaceFuturesTriggerOrderRequest request, CancellationToken ct)
        {
            var side = GetOrderSide(request);
            var validationError = PlaceFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                side,
                request.OrderPrice == null ? NewOrderType.StopMarket : NewOrderType.StopLimit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts,
                triggerPrice: request.TriggerPrice,
                price: request.OrderPrice,
                clientOrderId: request.ClientOrderId,
                positionSide: request.PositionSide.ToPositionSide(),
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        private OrderSide GetOrderSide(PlaceFuturesTriggerOrderRequest request)
        {
            if (request.PositionSide == SharedPositionSide.Long)
                return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Buy : OrderSide.Sell;
        
            return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Sell : OrderSide.Buy;
        }

        public GetFuturesTriggerOrderOptions GetFuturesTriggerOrderOptions { get; } = new GetFuturesTriggerOrderOptions(_exchange, true)
        {
        };
        public async Task<HttpResult<SharedFuturesTriggerOrder>> GetFuturesTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesTriggerOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var openOrders = await _api.Trading.GetOpenOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesTriggerOrder>(openOrders);

            var openOrder = openOrders.Data.SingleOrDefault();
            if (openOrder != null)
            {
                return HttpResult.Ok(openOrders, new SharedFuturesTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, openOrder.Symbol),
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
                    FeeAsset = openOrder.FeeAsset,
                    ClientOrderId = openOrder.ClientOrderId
                });
            }
            else
            {
                var closeOrders = await _api.Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                if (!closeOrders.Success)
                    return HttpResult.Fail<SharedFuturesTriggerOrder>(closeOrders);

                if (!closeOrders.Data.Any())
                    return HttpResult.Fail<SharedFuturesTriggerOrder>(closeOrders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                var closedOrder = closeOrders.Data.Single().Value.Single();
                
                return HttpResult.Ok(closeOrders, new SharedFuturesTriggerOrder(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, closedOrder.Symbol),
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
                    Fee = closedOrder.Fee,
                    FeeAsset = closedOrder.FeeAsset
                });
            }
        }

        private SharedTriggerOrderStatus ParseTriggerOrderStatus(WhiteBitClosedOrder closedOrder)
        {
            if (closedOrder.Status == OrderStatus.Open)
                return SharedTriggerOrderStatus.Active;

            if (closedOrder.Status == OrderStatus.Filled)
                return SharedTriggerOrderStatus.Filled;

            if (closedOrder.Status == OrderStatus.Canceled || closedOrder.Status == OrderStatus.AutoCanceledUserMargin)
                return SharedTriggerOrderStatus.CanceledOrRejected;

            return SharedTriggerOrderStatus.Unknown;
        }

        public CancelFuturesTriggerOrderOptions CancelFuturesTriggerOrderOptions { get; } = new CancelFuturesTriggerOrderOptions(_exchange, true);
        public async Task<HttpResult<SharedId>> CancelFuturesTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));
        }

        #endregion

        #region Tp/SL Client
        public SetFuturesTpSlOptions SetFuturesTpSlOptions { get; } = new SetFuturesTpSlOptions(_exchange, true)
        {
            RequiredRequestParameters = [
                RequestParameter<SetTpSlRequest>.Required(x => x.Quantity, "Quantity of the position to close, required by API", 0.123m)
                ]
        };

        public async Task<HttpResult<SharedId>> SetFuturesTpSlAsync(SetTpSlRequest request, CancellationToken ct)
        {
            var validationError = SetFuturesTpSlOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.CollateralTrading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                NewOrderType.StopMarket,
                quantity: request.Quantity!.Value,
                triggerPrice: request.TriggerPrice,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(result.Data.OrderId.ToString()));
        }

        public CancelFuturesTpSlOptions CancelFuturesTpSlOptions { get; } = new CancelFuturesTpSlOptions(_exchange, true)
        {
            RequiredRequestParameters = [
                RequestParameter<CancelTpSlRequest>.Required(x => x.OrderId, "Id of the tp/sl order", "123123")
                ]
        };

        public async Task<HttpResult<bool>> CancelFuturesTpSlAsync(CancelTpSlRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesTpSlOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<bool>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<bool>(Exchange, ArgumentError.Invalid(nameof(CancelTpSlRequest.OrderId), "Invalid order id"));

            var result = await _api.Trading.CancelOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                orderId,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<bool>(result);

            // Return
            return HttpResult.Ok(result, true);
        }

        #endregion

        #region Funding Rate client
        public GetFundingRateHistoryOptions GetFundingRateHistoryOptions { get; } = new GetFundingRateHistoryOptions(_exchange, false, true, true, 100, false);

        public async Task<HttpResult<SharedFundingRate[]>> GetFundingRateHistoryAsync(GetFundingRateHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetFundingRateHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFundingRate[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.ExchangeData.GetFundingHistoryAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                offset: pageParams.Offset,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFundingRate[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                 () => Pagination.NextPageFromOffset(pageParams, limit),
                 result.Data.Length,
                 result.Data.Select(x => x.FundingTime),
                 request.StartTime,
                 request.EndTime ?? DateTime.UtcNow,
                 pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.FundingTime, request.StartTime, request.EndTime, direction)
                       .Select(x => 
                           new SharedFundingRate(x.FundingRate, x.FundingTime))
                       .ToArray(), nextPageRequest);
        }
        #endregion

        #region Transfer client

        public TransferOptions TransferOptions { get; } = new TransferOptions(_exchange, [
            SharedAccountType.Spot,
            SharedAccountType.PerpetualLinearFutures,
            SharedAccountType.PerpetualInverseFutures,
            SharedAccountType.DeliveryLinearFutures,
            SharedAccountType.DeliveryInverseFutures
            ]);
        public async Task<HttpResult<SharedId>> TransferAsync(TransferRequest request, CancellationToken ct)
        {
            var validationError = TransferOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var fromType = GetTransferType(request.FromAccountType);
            var toType = GetTransferType(request.ToAccountType);
            if (fromType == null || toType == null)
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid("To/From AccountType", "invalid to/from account combination"));

            // Get data
            var transfer = await _api.Account.TransferAsync(
                fromType.Value,
                toType.Value,
                request.Asset,
                request.Quantity,
                ct: ct).ConfigureAwait(false);
            if (!transfer.Success)
                return HttpResult.Fail<SharedId>(transfer);

            return HttpResult.Ok(transfer, new SharedId(""));
        }

        private AccountType? GetTransferType(SharedAccountType type)
        {
            if (type == SharedAccountType.Funding) return AccountType.Main;
            if (type == SharedAccountType.Spot) return AccountType.Spot;
            if (type.IsFuturesAccount()) return AccountType.Collateral;
            return null;
        }

        #endregion
    }
}

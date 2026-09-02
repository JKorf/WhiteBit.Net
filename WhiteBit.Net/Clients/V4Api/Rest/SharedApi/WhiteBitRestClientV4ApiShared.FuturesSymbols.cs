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
    internal partial class WhiteBitRestClientV4SharedApi
    {
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
    }
}

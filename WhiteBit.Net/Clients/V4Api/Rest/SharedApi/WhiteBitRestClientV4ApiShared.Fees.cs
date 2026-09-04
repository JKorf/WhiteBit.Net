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
        #region Get Fees

        async Task<ICallResult<SharedFee>> IGetFees.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
            => await GetFeesAsync(request, ct).ConfigureAwait(false);

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
    }
}

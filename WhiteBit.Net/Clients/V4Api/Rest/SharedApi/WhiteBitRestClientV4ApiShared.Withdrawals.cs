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

        #region Get Withdrawal History

        async Task<ICallResult<SharedWithdrawal[]>> IGetWithdrawalHistory.GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetWithdrawalHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

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

        #endregion

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
        #region Withdraw

        async Task<ICallResult<SharedId>> IWithdraw.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
            => await WithdrawAsync(request, ct).ConfigureAwait(false);

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
    }
}

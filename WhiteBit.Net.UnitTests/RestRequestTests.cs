using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateV4AccountCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/Account", "https://whitebit.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.V4Api.Convert.GetConvertEstimateAsync("123", "123", 0.1m, "123"), "GetConvertEstimate");
            await tester.ValidateAsync(client => client.V4Api.Convert.ConfirmConvertAsync("123"), "ConfirmConvert");
            await tester.ValidateAsync(client => client.V4Api.Convert.GetConvertHistoryAsync(), "GetConvertHistory");
            await tester.ValidateAsync(client => client.V4Api.Account.GetFiatDepositAddressAsync("123", "123", 0.1m, "123"), "GetFiatDepositAddress");
            await tester.ValidateAsync(client => client.V4Api.Account.WithdrawAsync("123", 0.1m, "123", "123", true), "Withdraw");
            await tester.ValidateAsync(client => client.V4Api.Account.TransferAsync(AccountType.Spot, AccountType.Main, "123", 0.1m), "Transfer"); ;
            await tester.ValidateAsync(client => client.V4Api.Account.GetDepositWithdrawalHistoryAsync(), "GetDepositWithdrawalHistory");
            await tester.ValidateAsync(client => client.V4Api.Account.CreateDepositAddressAsync("123"), "CreateDepositAddress");
            await tester.ValidateAsync(client => client.V4Api.Account.GetDepositWithdrawalSettingsAsync(), "GetDepositWithdrawalSettings");
            await tester.ValidateAsync(client => client.V4Api.Account.GetMiningRewardHistoryAsync(), "GetMiningRewardHistory");
            await tester.ValidateAsync(client => client.V4Api.Account.GetCollateralBalanceSummaryAsync(), "GetCollateralBalanceSummary");
            await tester.ValidateAsync(client => client.V4Api.Account.SetAccountLeverageAsync(123), "SetAccountLeverage");
            await tester.ValidateAsync(client => client.V4Api.Account.GetCollateralAccountSummaryAsync(), "GetCollateralAccountSummary");
            await tester.ValidateAsync(client => client.V4Api.Codes.CreateCodeAsync("123", 0.1m), "CreateCode");
            await tester.ValidateAsync(client => client.V4Api.Codes.ApplyCodeAsync("123"), "ApplyCode");
            await tester.ValidateAsync(client => client.V4Api.Codes.GetCreatedCodesAsync(), "GetCreatedCodes");
            await tester.ValidateAsync(client => client.V4Api.Codes.GetCodeHistoryAsync(), "GetCodeHistory");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.CreateSubAccountAsync("123"), "CreateSubAccount");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.DeleteSubAccountAsync("123"), "DeleteSubAccount");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.EditSubAccountAsync("123", "123", "123", "123"), "EditSubAccount");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.GetSubAccountsAsync(), "GetSubAccounts");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.SubaccountTransferAsync("123", SubTransferDirection.ToSubAccount, "123", 0.1m), "SubaccountTransfer");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.BlockSubaccountAsync("123"), "BlockSubaccount");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.UnblockSubaccountAsync("123"), "UnblockSubaccount");
            await tester.ValidateAsync(client => client.V4Api.SubAccount.GetSubaccountTransferHistoryAsync(), "GetSubaccountTransferHistory");
        }

        [Test]
        public async Task ValidateV4ExchangeDataCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/ExchangeData", "https://whitebit.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetSystemStatusAsync(), "GetSystemStatus");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetTickersAsync(), "GetTickers", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetOrderBookAsync("ETH_USDT"), "GetOrderBook");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetRecentTradesAsync("ETH_USDT"), "GetRecentTrades");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetCollateralSymbolsAsync(), "GetCollateralSymbols", nestedJsonProperty: "result");
            await tester.ValidateAsync(client => client.V4Api.ExchangeData.GetFuturesSymbolsAsync(), "GetFuturesSymbols", nestedJsonProperty: "result");
        }

        [Test]
        public async Task ValidateV4TradingCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/Trading", "https://whitebit.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.V4Api.Trading.GetUserTradesAsync("ETH_USDT"), "GetUserTrades");
            await tester.ValidateAsync(client => client.V4Api.Trading.GetOrderTradesAsync(123), "GetOrderTrades", nestedJsonProperty: "records");
            await tester.ValidateAsync(client => client.V4Api.Trading.GetClosedOrdersAsync(), "GetClosedOrders", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.V4Api.Trading.GetClosedOrdersAsync(), "GetClosedOrders2");
        }

        [Test]
        public async Task ValidateV4CollateralTradingCalls()
        {
            var client = new WhiteBitRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<WhiteBitRestClient>(client, "Endpoints/V4/CollateralTrading", "https://whitebit.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.V4Api.CollateralTrading.GetOpenPositionsAsync("123"), "GetOpenPositions");
            await tester.ValidateAsync(client => client.V4Api.CollateralTrading.GetPositionHistoryAsync(), "GetPositionHistory");
            await tester.ValidateAsync(client => client.V4Api.CollateralTrading.GetOpenConditionalOrdersAsync("123"), "GetOpenConditionalOrders");
            await tester.ValidateAsync(client => client.V4Api.CollateralTrading.PlaceOcoOrderAsync("123", OrderSide.Sell, 0.1m, 0.1m, 0.1m, 0.1m), "PlaceOcoOrder");
            await tester.ValidateAsync(client => client.V4Api.CollateralTrading.CancelOcoOrderAsync("123", 123), "CancelOcoOrder");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestHeaders?.Any(x => x.Key == "X-TXC-SIGNATURE") == true;
        }
    }
}

using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using WhiteBit.Net.Interfaces.Clients.V4Api;
using CryptoExchange.Net.Objects.Sockets;
using System.Linq;
using CryptoExchange.Net.Objects;
using WhiteBit.Net.Enums;
using WhiteBit.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;

namespace WhiteBit.Net.Clients.V4Api
{
    internal partial class WhiteBitSocketClientV4SharedApi
    {
        #region Balance client
        public SubscribeBalanceOptions SubscribeBalanceOptions { get; } = new SubscribeBalanceOptions(_exchange, true)
        {
            OptionalExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("BalanceAssets", typeof(List<string>), "The assets to subscribe for updates", new List<string>{ "USDT", "ETH", "BTC" })
            }
        };
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
                if (assets == null)
                {
                    // request all assets
                    var client = new WhiteBitRestClient(x =>
                    {
                        x.Environment = _api.ClientOptions.Environment;
                    });
                    var assetsResult = await client.V4Api.ExchangeData.GetAssetsAsync().ConfigureAwait(false);
                    if (!assetsResult.Success)
                        return WebSocketResult.Fail<UpdateSubscription>(Exchange, assetsResult.Error!);

                    assets = assetsResult.Data.Where(x => x.CanDeposit).Select(x => x.Asset).ToList();
                }

                var result = await _api.SubscribeToSpotBalanceUpdatesAsync(
                    assets!,
                    update => handler(update.ToType<SharedBalance[]>(update.Data.Select(x =>
                        new SharedBalance(TradingMode.Spot, x.Key, x.Value.Available, x.Value.Available + x.Value.Frozen)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var assets = ExchangeParameters.GetValue<List<string>>(request.ExchangeParameters, Exchange, "BalanceAssets");
                if (assets == null)
                {
                    // request all assets
                    var client = new WhiteBitRestClient(x =>
                    {
                        x.Environment = _api.ClientOptions.Environment;
                        x.ApiCredentials = (WhiteBitCredentials?)_api.AuthenticationProvider!.ApiCredentials.Copy();
                    });
                    var assetsResult = await client.V4Api.Account.GetCollateralBalancesAsync().ConfigureAwait(false);
                    if (!assetsResult.Success)
                        return WebSocketResult.Fail<UpdateSubscription>(Exchange, assetsResult.Error!);

                    assets = assetsResult.Data.Select(x => x.Key).Distinct().ToList();
                }

                var result = await _api.SubscribeToMarginBalanceUpdatesAsync(
                    assets,
                    update => handler(update.ToType<SharedBalance[]>(update.Data.Select(x => 
                        new SharedBalance(
                            [TradingMode.PerpetualLinear], x.Asset, x.AvailableWithoutBorrow, x.Balance)).ToArray())),
                    ct: ct).ConfigureAwait(false);
                return result;
            }
        }

        #endregion
    }
}

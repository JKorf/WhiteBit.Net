// 04-multi-exchange.cs
//
// Demonstrates: writing exchange-agnostic code using CryptoExchange.Net.SharedApis.
// Same pattern works against WhiteBit and other CryptoExchange.Net exchange libraries.
//
// Setup:
//   dotnet add package WhiteBit.Net
//   dotnet add package JK.OKX.Net    // optional, for the OKX example
//   dotnet add package Binance.Net   // optional, for the Binance example

using CryptoExchange.Net.SharedApis;
using WhiteBit.Net.Clients;

// ---- THE PATTERN ----
// WhiteBit exposes a `.SharedClient` property on V4Api.
// SharedClient implements interfaces like ISpotTickerRestClient, ISpotOrderRestClient,
// IBalanceRestClient, and more: a common abstraction across exchange libraries.
// Use SharedClient.Discover() when you need runtime capability metadata.

ISpotTickerRestClient whiteBitShared = new WhiteBitRestClient().V4Api.SharedClient;

// To add OKX or Binance, install the package and:
//   ISpotTickerRestClient okxShared     = new OKXRestClient().UnifiedApi.SharedClient;
//   ISpotTickerRestClient binanceShared = new BinanceRestClient().SpotApi.SharedClient;

// SharedSymbol normalizes exchange-specific symbol formatting.
// WhiteBit direct calls use "ETH_USDT", but shared APIs use base/quote components.
var ethUsdt = new SharedSymbol(TradingMode.Spot, "ETH", "USDT");

await PrintTicker(whiteBitShared, ethUsdt);
// await PrintTicker(okxShared, ethUsdt);
// await PrintTicker(binanceShared, ethUsdt);

// ---- AGNOSTIC METHOD ----
async Task PrintTicker(ISpotTickerRestClient client, SharedSymbol symbol)
{
    var result = await client.GetSpotTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
    {
        Console.WriteLine($"[{client.Exchange}] Failed: {result.Error}");
        return;
    }

    Console.WriteLine($"[{client.Exchange}] {result.Data.Symbol}: {result.Data.LastPrice}");
}

// ---- AVAILABLE SHARED INTERFACES ----
// REST examples:
//   ISpotTickerRestClient, ISpotSymbolRestClient, ISpotOrderRestClient
//   ISpotTriggerOrderRestClient, IFuturesOrderRestClient, IFuturesSymbolRestClient
//   IFuturesTriggerOrderRestClient, IFuturesTpSlRestClient, IBalanceRestClient
//   IAssetsRestClient, IDepositRestClient, IWithdrawalRestClient, IWithdrawRestClient
//   IFeeRestClient, IOrderBookRestClient, IRecentTradeRestClient, IBookTickerRestClient
//   IFundingRateRestClient, ITransferRestClient, ILeverageRestClient
// WebSocket examples:
//   ITickerSocketClient, IBookTickerSocketClient, IOrderBookSocketClient
//   ITradeSocketClient, IKlineSocketClient, IUserTradeSocketClient
//   IBalanceSocketClient, ISpotOrderSocketClient, IFuturesOrderSocketClient
//   IPositionSocketClient

// ---- WEBSOCKET EXAMPLE: SHARED SUBSCRIPTION ----
// Shared socket subscriptions return WebSocketResult<UpdateSubscription>.
var whiteBitSocket = new WhiteBitSocketClient();
ITickerSocketClient whiteBitTickerSocket = whiteBitSocket.V4Api.SharedClient;

var sub = await whiteBitTickerSocket.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(ethUsdt),
    update => Console.WriteLine($"[{whiteBitTickerSocket.Exchange}] {update.Data.Symbol}: {update.Data.LastPrice}"));

if (!sub.Success)
{
    Console.WriteLine($"Subscribe failed: {sub.Error}");
    return;
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

// Shared socket interfaces do not expose UnsubscribeAsync. Keep the concrete socket client.
await whiteBitSocket.UnsubscribeAsync(sub.Data);

// Common variations:
//   Multi-exchange scanner: loop over List<ISpotTickerRestClient>
//   Cross-exchange order book: IOrderBookSocketClient on each exchange
//   Unified balances: IBalanceRestClient on each exchange
//   Best execution: ISpotOrderRestClient or IFuturesOrderRestClient per venue

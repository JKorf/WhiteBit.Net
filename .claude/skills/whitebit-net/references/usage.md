# Usage examples

Install `WhiteBit.Net`. Each C# block is independent and includes its imports. Trading blocks define functions without invoking them; compilation never submits orders. Use [API surfaces](api-surfaces.md) for routing and read [operational considerations](safety.md) before account-changing calls.

## Public setup and results

```csharp
using WhiteBit.Net.Clients;

using var client = new WhiteBitRestClient();
var result = await client.V4Api.ExchangeData.GetTickersAsync();
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}
var ticker = result.Data.SingleOrDefault(x => x.Symbol == "ETH_USDT");
if (ticker is not null)
    Console.WriteLine(ticker.LastPrice);
```

## Credentials and account reads

WhiteBit credentials require a key and secret, without a passphrase.

```csharp
using WhiteBit.Net;
using WhiteBit.Net.Clients;

var key = Environment.GetEnvironmentVariable("WHITEBIT_API_KEY")
    ?? throw new InvalidOperationException("Missing API key");
var secret = Environment.GetEnvironmentVariable("WHITEBIT_API_SECRET")
    ?? throw new InvalidOperationException("Missing API secret");
using var client = new WhiteBitRestClient(options =>
    options.ApiCredentials = new WhiteBitCredentials(key, secret));
var balances = await client.V4Api.Account.GetSpotBalancesAsync();
if (!balances.Success)
    Console.WriteLine(balances.Error);
else
    Console.WriteLine(balances.Data);
var positions = await client.V4Api.CollateralTrading.GetOpenPositionsAsync("ETH_PERP");
if (!positions.Success)
    Console.WriteLine(positions.Error);
else
    Console.WriteLine($"Open positions: {positions.Data.Length}");
```

## Spot order function

Supply an authenticated client and validated symbol, quantity, and price when invoking this function. It submits an order and does not assume immediate fill or automatic cancellation.

```csharp
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

static async Task SubmitSpotLimitAsync(
    WhiteBitRestClient client, string symbol, decimal quantity, decimal price, CancellationToken ct)
{
    var result = await client.V4Api.Trading.PlaceSpotOrderAsync(
        symbol, OrderSide.Buy, NewOrderType.Limit,
        quantity: quantity, price: price, ct: ct);
    if (!result.Success)
        Console.WriteLine(result.Error);
    else
        Console.WriteLine($"Submitted order {result.Data.OrderId}");
}
```

## Collateral order function

This function does not change account leverage or hedge mode. Supply the intended position leg if required by the configured mode. Native spot and collateral order models are distinct.

```csharp
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

static async Task SubmitCollateralLimitAsync(
    WhiteBitRestClient client, string symbol, OrderSide side, decimal quantity,
    decimal price, PositionSide? positionSide, bool reduceOnly, CancellationToken ct)
{
    var result = await client.V4Api.CollateralTrading.PlaceOrderAsync(
        symbol, side, NewOrderType.Limit, quantity: quantity, price: price,
        positionSide: positionSide, reduceOnly: reduceOnly, ct: ct);
    if (!result.Success)
        Console.WriteLine(result.Error);
    else
        Console.WriteLine(result.Data.OrderId);
}
```

## Socket request and public subscription

The query has its own result; the subscription remains active until explicitly closed.

```csharp
using WhiteBit.Net.Clients;

using var socket = new WhiteBitSocketClient();
var query = await socket.V4Api.GetTickerAsync("ETH_USDT");
if (!query.Success)
    Console.WriteLine(query.Error);
else
    Console.WriteLine(query.Data);

var subscription = await socket.V4Api.SubscribeToTickerUpdatesAsync(
    "ETH_USDT", update => Console.WriteLine(update.Data.Ticker.LastPrice));
if (!subscription.Success)
{
    Console.WriteLine(subscription.Error);
    return;
}
try
{
    await Task.Delay(TimeSpan.FromSeconds(30));
}
finally
{
    await socket.UnsubscribeAsync(subscription.Data);
}
```

## Private subscription

```csharp
using WhiteBit.Net;
using WhiteBit.Net.Clients;

var key = Environment.GetEnvironmentVariable("WHITEBIT_API_KEY")
    ?? throw new InvalidOperationException("Missing API key");
var secret = Environment.GetEnvironmentVariable("WHITEBIT_API_SECRET")
    ?? throw new InvalidOperationException("Missing API secret");
using var socket = new WhiteBitSocketClient(options =>
    options.ApiCredentials = new WhiteBitCredentials(key, secret));
var subscription = await socket.V4Api.SubscribeToOpenOrderUpdatesAsync(
    new[] { "ETH_USDT" }, update => Console.WriteLine(update.Data.Order.Status));
if (!subscription.Success)
{
    Console.WriteLine(subscription.Error);
    return;
}
try
{
    await Task.Delay(TimeSpan.FromSeconds(30));
}
finally
{
    await socket.UnsubscribeAsync(subscription.Data);
}
```

## Shared API V2

```csharp
using CryptoExchange.Net.SharedApis;
using WhiteBit.Net.Clients;

using var client = new WhiteBitRestClient();
IGetTickerRest ticker = client.V4Api.SharedApi;
var symbol = new SharedSymbol(TradingMode.Spot, "ETH", "USDT");
var result = await ticker.GetTickerAsync(new GetTickerRequest(symbol));
if (!result.Success)
    Console.WriteLine($"{ticker.Exchange}: {result.Error}");
else
    Console.WriteLine(result.Data.LastPrice);
```

## DI and runtime selection

The container owns the clients it creates. With several exchanges registered, select the exchange aggregate rather than relying on a single common capability registration.

```csharp
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.DependencyInjection;
using WhiteBit.Net;
using WhiteBit.Net.Interfaces.Clients;

var services = new ServiceCollection();
services.AddWhiteBit(options => options.Environment = WhiteBitEnvironment.Live);
using var provider = services.BuildServiceProvider();
var shared = provider.GetRequiredService<IWhiteBitSharedApiClient>();
IGetTickerRest knownSurface = shared.V4Rest;
var match = shared.GetCapability(
    SharedCapabilities.Tickers.GetTicker.Rest, TradingMode.Spot);
if (match is null)
    return;
Console.WriteLine(match.Options);
var result = await match.Capability.GetTickerAsync(
    new GetTickerRequest(new SharedSymbol(TradingMode.Spot, "ETH", "USDT")));
if (!result.Success)
    Console.WriteLine(result.Error);
else
    Console.WriteLine(result.Data.LastPrice);
```

Checkout-only: [standalone programs](../../../../Examples/ai-friendly/README.md) and [complete projects](../../../../Examples/README.md). Compile them without running to validate usage without exchange calls. Some programs place live orders when executed with valid credentials.

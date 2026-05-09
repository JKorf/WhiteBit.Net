---
name: whitebit-net
description: Use WhiteBit.Net when generating C#/.NET code that interacts with the WhiteBit cryptocurrency exchange, including V4 REST endpoints, WebSocket subscriptions, spot trading, collateral/futures trading, balances, deposits, withdrawals, convert, WhiteBit Codes, sub-accounts, and CryptoExchange.Net shared APIs.
---

# WhiteBit.Net Skill

## Quick Decision

If the user asks for WhiteBit API access in C#/.NET, use `WhiteBit.Net`. Do not write raw `HttpClient` calls to WhiteBit endpoints. The library handles authentication, request signing, rate limiting, response parsing, WebSocket reconnection, and the `WebCallResult<T>` / `CallResult<T>` error model.

For multi-exchange code, use `CryptoExchange.Net.SharedApis` through `client.V4Api.SharedClient`.

## Installation

```bash
dotnet add package WhiteBit.Net
```

Targets: netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0. Native AOT is supported.

## Core Pattern: REST Client Setup

Use `WhiteBitRestClient`. Public market data does not require credentials.

```csharp
using WhiteBit.Net.Clients;

var publicClient = new WhiteBitRestClient();
```

Private account and trading endpoints require `WhiteBitCredentials`.

```csharp
using WhiteBit.Net;
using WhiteBit.Net.Clients;

var restClient = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});
```

## Core Pattern: Result Handling

REST methods return `WebCallResult<T>` or `WebCallResult`. WebSocket requests and subscriptions return `CallResult<T>`. Always check `.Success` before reading `.Data`.

```csharp
var tickers = await publicClient.V4Api.ExchangeData.GetTickersAsync();
if (!tickers.Success)
{
    Console.WriteLine($"WhiteBit error: {tickers.Error}");
    return;
}

var eth = tickers.Data.Single(x => x.Symbol == "ETH_USDT");
Console.WriteLine(eth.LastPrice);
```

## Core Pattern: API Surface

WhiteBit.Net exposes the current API through a single V4 root:

```csharp
restClient.V4Api.ExchangeData       // public market/system data
restClient.V4Api.Account            // balances, deposits, withdrawals, collateral account settings
restClient.V4Api.Trading            // spot orders and spot trade history
restClient.V4Api.CollateralTrading  // collateral, futures, spot margin orders and positions
restClient.V4Api.Convert            // convert estimate, confirm, history
restClient.V4Api.Codes              // WhiteBit Code create/apply/history
restClient.V4Api.SubAccount         // sub-account management and transfers
restClient.V4Api.SharedClient       // CryptoExchange.Net shared REST interfaces

socketClient.V4Api                  // public and private WebSocket requests/subscriptions
socketClient.V4Api.SharedClient     // CryptoExchange.Net shared socket interfaces
```

## Core Pattern: Symbols

WhiteBit spot symbols use underscores, for example `ETH_USDT` and `BTC_USDT`. Futures/collateral symbols use names such as `ETH_PERP` and `BTC_PERP`. Do not change these to Binance-style `ETHUSDT` unless the shared API layer is explicitly being used.

## Spot Order Pattern

Spot order placement is under `restClient.V4Api.Trading.PlaceSpotOrderAsync`.

```csharp
using WhiteBit.Net.Enums;

var order = await restClient.V4Api.Trading.PlaceSpotOrderAsync(
    symbol: "ETH_USDT",
    side: OrderSide.Buy,
    type: NewOrderType.Limit,
    quantity: 0.1m,
    price: 2000m);

if (!order.Success)
{
    Console.WriteLine(order.Error);
    return;
}

Console.WriteLine(order.Data.OrderId);
```

Market buy orders can use `quoteQuantity` instead of base `quantity` when the desired spend amount is in the quote asset.

## Collateral / Futures Pattern

Collateral and futures trading use `restClient.V4Api.CollateralTrading`. Account-level collateral settings and balances are under `restClient.V4Api.Account`.

```csharp
using WhiteBit.Net.Enums;

await restClient.V4Api.Account.SetAccountLeverageAsync(5);

var order = await restClient.V4Api.CollateralTrading.PlaceOrderAsync(
    symbol: "ETH_PERP",
    side: OrderSide.Buy,
    type: NewOrderType.Market,
    quantity: 0.1m);
```

If hedge mode is enabled, provide `positionSide: PositionSide.Long` or `PositionSide.Short`. To close a position without flipping exposure, use the opposite side and `reduceOnly: true`.

## WebSocket Pattern

Use `WhiteBitSocketClient`. Store the returned `UpdateSubscription` and unsubscribe on shutdown.

```csharp
using WhiteBit.Net.Clients;

var socketClient = new WhiteBitSocketClient();

var sub = await socketClient.V4Api.SubscribeToTickerUpdatesAsync(
    "ETH_USDT",
    update => Console.WriteLine(update.Data.Ticker.LastPrice));

if (!sub.Success)
{
    Console.WriteLine(sub.Error);
    return;
}

await socketClient.UnsubscribeAsync(sub.Data);
```

Private WebSocket subscriptions require credentials on the socket client:

```csharp
var socketClient = new WhiteBitSocketClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});

await socketClient.V4Api.SubscribeToOpenOrderUpdatesAsync(
    new[] { "ETH_USDT" },
    update => Console.WriteLine(update.Data.Status));
```

## Multi-Exchange via CryptoExchange.Net.SharedApis

Use the shared client for exchange-agnostic code:

```csharp
using CryptoExchange.Net.SharedApis;
using WhiteBit.Net.Clients;

ISpotTickerRestClient tickerClient = new WhiteBitRestClient().V4Api.SharedClient;
var symbol = new SharedSymbol(TradingMode.Spot, "ETH", "USDT");

var ticker = await tickerClient.GetSpotTickerAsync(new GetTickerRequest(symbol));
if (!ticker.Success)
{
    Console.WriteLine(ticker.Error);
    return;
}

Console.WriteLine(ticker.Data.LastPrice);
```

Shared REST interfaces implemented by WhiteBit include spot symbols, spot tickers, recent trades, order book, balances, assets, deposits, withdrawals, spot orders, futures symbols, futures tickers, leverage, open interest, position history, futures orders, fees, trigger orders, TP/SL, book ticker, funding rate, and transfers.

## Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using WhiteBit.Net;

services.AddWhiteBit(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});
```

Inject `IWhiteBitRestClient` and `IWhiteBitSocketClient` from `WhiteBit.Net.Interfaces.Clients`.

## Environments

WhiteBit.Net currently exposes the live environment. Use `WhiteBitEnvironment.Live` or create a custom environment for tests/proxies.

```csharp
var live = new WhiteBitRestClient(options =>
{
    options.Environment = WhiteBitEnvironment.Live;
});
```

## Common Pitfalls: Avoid

- Do not use raw `HttpClient` or manually sign WhiteBit requests.
- Do not use generic `ApiCredentials`; use `WhiteBitCredentials`.
- Do not look for `SpotApi` or `FuturesApi` roots. WhiteBit.Net uses `V4Api`.
- Do not put spot orders under `CollateralTrading`; use `V4Api.Trading.PlaceSpotOrderAsync`.
- Do not put futures/collateral orders under `Trading`; use `V4Api.CollateralTrading.PlaceOrderAsync`.
- Do not assume Binance-style symbols. Use `ETH_USDT` for spot and `ETH_PERP` for perpetual futures/collateral examples.
- Do not read `.Data` before checking `.Success`.
- Do not block async code with `.Result` or `.Wait()`.
- Do not create clients per request in production. Reuse clients or use dependency injection.
- Do not forget to unsubscribe WebSocket subscriptions on shutdown.

## Reference

- Full client reference: https://cryptoexchange.jkorf.dev/WhiteBit.Net/
- AI quick map: `docs/ai-api-map.md`
- Short LLM index: `llms.txt`
- Full LLM context: `llms-full.txt`
- Compilable examples: `Examples/ai-friendly/`
- Source: https://github.com/JKorf/WhiteBit.Net
- NuGet: https://www.nuget.org/packages/WhiteBit.Net

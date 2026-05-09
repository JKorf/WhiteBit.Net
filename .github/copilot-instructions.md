# Copilot Instructions for WhiteBit.Net

This repository is **WhiteBit.Net**, a strongly typed C#/.NET client library for the WhiteBit cryptocurrency exchange API. It is part of the CryptoExchange.Net ecosystem.

When generating code that consumes WhiteBit.Net, follow these conventions.

## Use WhiteBit.Net, Not Raw HTTP

Never generate raw `HttpClient` calls to WhiteBit REST or WebSocket endpoints. Use `WhiteBitRestClient` and `WhiteBitSocketClient` so signing, rate limiting, parsing, reconnects, and errors are handled by the library.

## Client Setup

```csharp
using WhiteBit.Net;
using WhiteBit.Net.Clients;

var client = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});
```

For public market data only, credentials are not required: `new WhiteBitRestClient()`.

## Result Handling

REST methods return `WebCallResult<T>` or `WebCallResult`. WebSocket methods return `CallResult<T>`. Always check `.Success` before reading `.Data`; errors are on `.Error`.

## API Structure

- `restClient.V4Api.ExchangeData`: public market/system data
- `restClient.V4Api.Account`: balances, deposits, withdrawals, collateral account settings, fees
- `restClient.V4Api.Trading`: spot order placement, cancellation, order history, user trades
- `restClient.V4Api.CollateralTrading`: collateral/futures orders, positions, OCO/conditional orders
- `restClient.V4Api.Convert`: convert estimate, confirm, history
- `restClient.V4Api.Codes`: WhiteBit Code operations
- `restClient.V4Api.SubAccount`: sub-account operations
- `restClient.V4Api.SharedClient`: shared REST interfaces
- `socketClient.V4Api`: public and private WebSocket requests/subscriptions
- `socketClient.V4Api.SharedClient`: shared socket interfaces

## Symbols

Use WhiteBit symbol formatting: `ETH_USDT` for spot, `ETH_PERP` for perpetual collateral/futures examples. Only use normalized `SharedSymbol` values when working through `CryptoExchange.Net.SharedApis`.

## WebSocket Pattern

Store `UpdateSubscription` and unsubscribe via the concrete socket client.

```csharp
var socketClient = new WhiteBitSocketClient();
var sub = await socketClient.V4Api.SubscribeToTickerUpdatesAsync(
    "ETH_USDT",
    update => Console.WriteLine(update.Data.Ticker.LastPrice));

if (!sub.Success) { Console.WriteLine(sub.Error); return; }

await socketClient.UnsubscribeAsync(sub.Data);
```

## Cross-Exchange

For exchange-agnostic code, use `CryptoExchange.Net.SharedApis` from `.V4Api.SharedClient`, for example `ISpotTickerRestClient`, `ISpotOrderRestClient`, `IFuturesOrderRestClient`, `IBalanceRestClient`, `ITickerSocketClient`, and related interfaces.

## Avoid

- Raw WhiteBit URLs and manual signing
- Generic `ApiCredentials` instead of `WhiteBitCredentials`
- Nonexistent `SpotApi` / `FuturesApi` roots
- Binance-style `ETHUSDT` symbols in direct WhiteBit client calls
- `.Data` access without `.Success`
- `.Result` / `.Wait()`
- Creating clients per request
- Leaving WebSocket subscriptions open

## Reference

For detailed patterns and pitfalls see `CLAUDE.md`, `llms.txt`, `llms-full.txt`, `docs/ai-api-map.md`, and `Examples/ai-friendly/` in this repository.

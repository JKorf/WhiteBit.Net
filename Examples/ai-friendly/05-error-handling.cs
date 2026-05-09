// 05-error-handling.cs
//
// Demonstrates: WebCallResult patterns, retry logic, common WhiteBit routing
// and validation scenarios.
//
// Setup: dotnet add package WhiteBit.Net

using CryptoExchange.Net.Objects;
using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

var client = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});

// ---- 1. THE BASIC PATTERN ----
// Every REST method returns WebCallResult<T> or WebCallResult.
// .Success is true/false. .Data is valid only when .Success is true.
// .Error contains structured error info when .Success is false.

var result = await client.V4Api.ExchangeData.GetTickersAsync();

if (result.Success)
{
    var eth = result.Data.SingleOrDefault(x => x.Symbol == "ETH_USDT");
    Console.WriteLine($"Price: {eth?.LastPrice}");
}
else
{
    Console.WriteLine($"Code:      {result.Error?.Code}");
    Console.WriteLine($"Message:   {result.Error?.Message}");
    Console.WriteLine($"Type:      {result.Error?.ErrorType}");
    Console.WriteLine($"Transient: {result.Error?.IsTransient}");
}

// ---- 2. SIMPLE RETRY WITH BACKOFF ----
// Retry only on transient errors: network blips, temporary server errors, rate limits.
// Do not retry validation errors, permission errors, or insufficient balance.

async Task<WebCallResult<T>> WithRetry<T>(
    Func<Task<WebCallResult<T>>> call,
    int maxAttempts = 3)
{
    WebCallResult<T> last = default!;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        last = await call();
        if (last.Success)
            return last;

        if (last.Error?.IsTransient != true)
            return last;

        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, attempt)));
    }

    return last;
}

var tickers = await WithRetry(
    () => client.V4Api.ExchangeData.GetTickersAsync());

if (!tickers.Success)
{
    Console.WriteLine($"Ticker retry failed: {tickers.Error}");
}

// ---- 3. COMMON WHITEBIT ERROR SCENARIOS ----
//
// Authentication/signature errors:
//   Check WhiteBitCredentials, API key permissions, environment, and timestamp/nonce handling.
//
// Invalid symbol:
//   Direct WhiteBit calls use ETH_USDT for spot and ETH_PERP for perpetual/collateral symbols.
//   Do not use Binance-style ETHUSDT unless using SharedApis with SharedSymbol.
//
// Invalid order parameters:
//   NewOrderType.Limit needs quantity and price.
//   NewOrderType.Market needs quantity, or quoteQuantity for market buys where supported.
//   NewOrderType.StopLimit / StopMarket need triggerPrice.
//
// Insufficient balance:
//   Permanent for the attempted order. Surface it to the caller rather than retrying.
//
// Unknown order:
//   The order may already be filled/canceled, or the symbol/order id pair may be wrong.

// ---- 4. SYMBOL METADATA BEFORE ORDER PLACEMENT ----
var symbols = await client.V4Api.ExchangeData.GetSymbolsAsync();
if (!symbols.Success)
{
    Console.WriteLine($"Cannot fetch symbols: {symbols.Error}");
    return;
}

var symbol = symbols.Data.SingleOrDefault(x => x.Name == "ETH_USDT");
if (symbol == null || !symbol.TradingEnabled)
{
    Console.WriteLine("ETH_USDT is not currently tradable.");
    return;
}

var rawQuantity = 0.01m;
var rawPrice = 2000m;

if (rawQuantity < symbol.MinOrderQuantity)
{
    Console.WriteLine($"Quantity is below minimum: {symbol.MinOrderQuantity}");
    return;
}

if (rawQuantity * rawPrice < symbol.MinOrderValue)
{
    Console.WriteLine($"Order value is below minimum: {symbol.MinOrderValue}");
    return;
}

var order = await client.V4Api.Trading.PlaceSpotOrderAsync(
    symbol: "ETH_USDT",
    side: OrderSide.Buy,
    type: NewOrderType.Limit,
    quantity: rawQuantity,
    price: rawPrice);

if (!order.Success)
{
    var category = order.Error?.IsTransient == true
        ? "Transient; retry with backoff"
        : "Permanent; surface to user";

    Console.WriteLine($"{category}: {order.Error?.Code} {order.Error?.Message}");
}

// ---- 5. EXCEPTIONS VS ERROR RESULTS ----
// WhiteBit.Net returns exchange and network errors via WebCallResult.Error.
// Exceptions are usually for misconfiguration, disposal, cancellation, or programming errors.

// Common variations:
//   With CancellationToken: pass `ct: cancellationToken` to any method
//   With timeout:          options.RequestTimeout = TimeSpan.FromSeconds(10)
//   SharedApis:            use the same .Success / .Error handling pattern

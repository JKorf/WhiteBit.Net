// 03-websocket.cs
//
// Demonstrates: WebSocket subscriptions: public ticker, klines,
// authenticated open orders and spot balances. Includes proper teardown.
//
// Setup: dotnet add package WhiteBit.Net

using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

// ---- 1. PUBLIC SOCKET CLIENT ----
// Reuse a single client instance across subscriptions.
var publicSocket = new WhiteBitSocketClient();

var tickerSub = await publicSocket.V4Api.SubscribeToTickerUpdatesAsync(
    "ETH_USDT",
    update =>
    {
        // Keep handlers fast; queue heavy work elsewhere.
        Console.WriteLine($"ETH: {update.Data.Ticker.LastPrice} (quote volume {update.Data.Ticker.QuoteVolume})");
    });

if (!tickerSub.Success)
{
    Console.WriteLine($"Failed to subscribe ticker: {tickerSub.Error}");
    return;
}

var klineSub = await publicSocket.V4Api.SubscribeToKlineUpdatesAsync(
    "ETH_USDT",
    KlineInterval.OneMinute,
    update =>
    {
        foreach (var kline in update.Data)
        {
            Console.WriteLine($"ETH 1m: O={kline.OpenPrice} H={kline.HighPrice} L={kline.LowPrice} C={kline.ClosePrice}");
        }
    });

if (!klineSub.Success)
{
    Console.WriteLine($"Failed to subscribe klines: {klineSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    return;
}

// ---- 2. AUTHENTICATED SOCKET CLIENT ----
// Private subscriptions require credentials and permissions.
var authSocket = new WhiteBitSocketClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});

var orderSub = await authSocket.V4Api.SubscribeToOpenOrderUpdatesAsync(
    new[] { "ETH_USDT" },
    update =>
    {
        var order = update.Data.Order;
        Console.WriteLine($"Order {order.OrderId} {order.Symbol}: {order.Status} ({order.QuantityFilled}/{order.Quantity})");
    });

if (!orderSub.Success)
{
    Console.WriteLine($"Failed to subscribe orders: {orderSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(klineSub.Data);
    return;
}

var balanceSub = await authSocket.V4Api.SubscribeToSpotBalanceUpdatesAsync(
    new[] { "ETH", "USDT" },
    update =>
    {
        foreach (var balance in update.Data)
            Console.WriteLine($"{balance.Key}: {balance.Value.Available} available, {balance.Value.Frozen} frozen");
    });

if (!balanceSub.Success)
{
    Console.WriteLine($"Failed to subscribe balances: {balanceSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(klineSub.Data);
    await authSocket.UnsubscribeAsync(orderSub.Data);
    return;
}

Console.WriteLine("All subscriptions active. Press Enter to teardown...");
Console.ReadLine();

// ---- 3. TEARDOWN ----
// Always unsubscribe on shutdown to release resources cleanly.
await publicSocket.UnsubscribeAsync(tickerSub.Data);
await publicSocket.UnsubscribeAsync(klineSub.Data);
await authSocket.UnsubscribeAsync(orderSub.Data);
await authSocket.UnsubscribeAsync(balanceSub.Data);

Console.WriteLine("Clean shutdown complete.");

// Common variations:
//   Multiple symbols:      SubscribeToTickerUpdatesAsync(new[] { "BTC_USDT", "ETH_USDT" }, handler)
//   Order book stream:     SubscribeToOrderBookUpdatesAsync("ETH_USDT", depth: 20, handler)
//   Trade stream:          SubscribeToTradeUpdatesAsync("ETH_USDT", handler)
//   Position stream:       SubscribeToPositionUpdatesAsync(handler)
//   User trade stream:     SubscribeToUserTradeUpdatesAsync(new[] { "ETH_USDT" }, handler)

// 01-spot-quickstart.cs
//
// Demonstrates: client setup, public market data, authenticated balances,
// spot limit order placement, open order query, cancellation.
//
// Setup:
//   dotnet new console -n SpotQuickstart && cd SpotQuickstart
//   dotnet add package WhiteBit.Net
//   Copy this file content into Program.cs
//   Substitute API_KEY / API_SECRET below
//   dotnet run

using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

// ---- 1. PUBLIC CLIENT (no credentials needed for market data) ----
// Reuse this client across the application; do not create a new client per request.
var publicClient = new WhiteBitRestClient();

var tickers = await publicClient.V4Api.ExchangeData.GetTickersAsync();
if (!tickers.Success)
{
    Console.WriteLine($"Failed to get tickers: {tickers.Error}");
    return;
}

var ethTicker = tickers.Data.SingleOrDefault(x => x.Symbol == "ETH_USDT");
if (ethTicker == null)
{
    Console.WriteLine("ETH_USDT ticker was not returned.");
    return;
}

Console.WriteLine($"ETH/USDT last price: {ethTicker.LastPrice}");
Console.WriteLine($"24h quote volume: {ethTicker.QuoteVolume} USDT");

// ---- 2. AUTHENTICATED CLIENT (for account / trading) ----
var tradingClient = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});

// Get spot trading balances. Use GetMainBalancesAsync for funding/main account balances.
var balances = await tradingClient.V4Api.Account.GetSpotBalancesAsync();
if (!balances.Success)
{
    Console.WriteLine($"Failed to get spot balances: {balances.Error}");
    return;
}

foreach (var balance in balances.Data.Where(b => b.Available + b.Frozen > 0))
{
    Console.WriteLine($"{balance.Asset}: {balance.Available} available, {balance.Frozen} frozen");
}

// ---- 3. PLACE A LIMIT BUY ORDER ----
// Direct WhiteBit spot symbols use underscores, for example ETH_USDT.
// Limit order price below the last price is less likely to fill immediately.
var safePrice = Math.Round(ethTicker.LastPrice * 0.95m, 2);

var order = await tradingClient.V4Api.Trading.PlaceSpotOrderAsync(
    symbol: "ETH_USDT",
    side: OrderSide.Buy,
    type: NewOrderType.Limit,
    quantity: 0.01m,
    price: safePrice);

if (!order.Success)
{
    Console.WriteLine($"Failed to place order: {order.Error}");
    return;
}

Console.WriteLine($"Placed order {order.Data.OrderId} at {safePrice}, status: {order.Data.Status}");

// ---- 4. QUERY OPEN ORDERS ----
// WhiteBit.Net exposes open-order lookup as a filtered list endpoint.
var openOrders = await tradingClient.V4Api.Trading.GetOpenOrdersAsync(
    symbol: "ETH_USDT",
    orderId: order.Data.OrderId);

if (openOrders.Success)
{
    var current = openOrders.Data.SingleOrDefault();
    Console.WriteLine(current == null
        ? "Order is no longer open."
        : $"Order status: {current.Status}, filled: {current.QuantityFilled}");
}

// ---- 5. CANCEL THE ORDER (cleanup for this example) ----
var cancel = await tradingClient.V4Api.Trading.CancelOrderAsync("ETH_USDT", order.Data.OrderId);
if (cancel.Success)
{
    Console.WriteLine($"Cancelled order {order.Data.OrderId}");
}

// Common variations:
//   Market order:           type: NewOrderType.Market, omit price
//   Market buy by notional: type: NewOrderType.Market, use quoteQuantity
//   Stop order:             type: NewOrderType.StopLimit or StopMarket, add triggerPrice
//   Main balances:          tradingClient.V4Api.Account.GetMainBalancesAsync()
//   Direct symbol format:   ETH_USDT, BTC_USDT, not ETHUSDT or BTCUSDT

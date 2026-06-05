// 02-collateral-futures.cs
//
// Demonstrates: WhiteBit collateral/futures trading: set leverage,
// place market order, retrieve open position, close with reduceOnly.
//
// Setup: dotnet add package WhiteBit.Net
// Substitute API_KEY / API_SECRET. The API key must have the required trading permissions.

using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

var client = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials("API_KEY", "API_SECRET");
});

const string symbol = "ETH_PERP";

// ---- 1. SET ACCOUNT LEVERAGE ----
// WhiteBit exposes leverage as an account-level collateral setting.
var leverage = await client.V4Api.Account.SetAccountLeverageAsync(5);
if (!leverage.Success)
{
    Console.WriteLine($"Failed to set leverage: {leverage.Error}");
    return;
}
Console.WriteLine($"Leverage response: {leverage.Data.Leverage}x");

// ---- 2. PLACE MARKET ORDER (open long exposure) ----
// In hedge mode add positionSide: PositionSide.Long.
var openOrder = await client.V4Api.CollateralTrading.PlaceOrderAsync(
    symbol: symbol,
    side: OrderSide.Buy,
    type: NewOrderType.Market,
    quantity: 0.1m);

if (!openOrder.Success)
{
    Console.WriteLine($"Failed to open position: {openOrder.Error}");
    return;
}
Console.WriteLine($"Opened position via order {openOrder.Data.OrderId}");

// ---- 3. GET CURRENT POSITION ----
var positions = await client.V4Api.CollateralTrading.GetOpenPositionsAsync(symbol);
if (!positions.Success)
{
    Console.WriteLine($"Failed to get positions: {positions.Error}");
    return;
}

var position = positions.Data.FirstOrDefault(p => p.Quantity != 0);
if (position == null)
{
    Console.WriteLine("No open position found. The order may not have filled yet.");
    return;
}

Console.WriteLine($"Position: {position.Quantity} {symbol} at base price {position.BasePrice}");
Console.WriteLine($"Unrealized PnL: {position.Pnl}");
Console.WriteLine($"Liquidation price: {position.LiquidationPrice}");

// ---- 4. CLOSE THE POSITION ----
// Opposite side, same quantity, reduceOnly=true to avoid accidentally flipping exposure.
var closeOrder = await client.V4Api.CollateralTrading.PlaceOrderAsync(
    symbol: symbol,
    side: OrderSide.Sell,
    type: NewOrderType.Market,
    quantity: Math.Abs(position.Quantity),
    positionSide: position.PositionSide,
    reduceOnly: true);

if (closeOrder.Success)
{
    Console.WriteLine($"Closed position via order {closeOrder.Data.OrderId}");
}

// Common variations:
//   Limit order:       type: NewOrderType.Limit, add price
//   Stop limit:        type: NewOrderType.StopLimit, add price and triggerPrice
//   Stop market:       type: NewOrderType.StopMarket, add triggerPrice
//   Hedge mode:        add positionSide: PositionSide.Long / PositionSide.Short
//   Hedge setting:     client.V4Api.Account.GetHedgeModeAsync() / SetHedgeModeAsync(...)
//   Collateral funds:  client.V4Api.Account.GetCollateralBalancesAsync()

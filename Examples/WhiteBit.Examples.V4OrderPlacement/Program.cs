using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

const string spotSymbol = "BTC_USDT";
const string futuresSymbol = "BTC_PERP";

// Replace with valid credentials or order placement will always fail
var apiKey = "KEY";
var apiSecret = "SECRET";

Console.WriteLine("WhiteBit.Net V4 order placement example");
Console.WriteLine();
Console.WriteLine("This example can place real orders when valid credentials are configured.");
Console.WriteLine();

var client = new WhiteBitRestClient(options =>
{
    options.ApiCredentials = new WhiteBitCredentials()
        .WithHMAC(apiKey, apiSecret);
});

await PlaceSpotLimitOrderAsync(client);
Console.WriteLine();
await PlaceFuturesReduceOnlyOrderExampleAsync(client);

static async Task PlaceSpotLimitOrderAsync(WhiteBitRestClient client)
{
    Console.WriteLine($"Placing spot V4 limit buy order for {spotSymbol}...");

    var ticker = await GetSpotTickerAsync(client, spotSymbol);
    if (ticker == null)
        return;

    var safePrice = Math.Round(ticker.Value * 0.95m, 2);
    var order = await client.V4Api.Trading.PlaceSpotOrderAsync(
        symbol: spotSymbol,
        side: OrderSide.Buy,
        type: NewOrderType.Limit,
        quantity: 0.001m,
        price: safePrice);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place spot order: {order.Error}");
        return;
    }

    Console.WriteLine($"Placed spot order {order.Data.OrderId}, status: {order.Data.Status}");

    var orderStatus = await client.V4Api.Trading.GetOpenOrdersAsync(spotSymbol, order.Data.OrderId);
    if (orderStatus.Success)
    {
        var openOrder = orderStatus.Data.SingleOrDefault();
        Console.WriteLine(openOrder == null
            ? "Spot order is no longer open"
            : $"Spot order status: {openOrder.Status}, filled: {openOrder.QuantityFilled}");
    }
    else
    {
        Console.WriteLine($"Failed to query spot order: {orderStatus.Error}");
    }

    var cancel = await client.V4Api.Trading.CancelOrderAsync(spotSymbol, order.Data.OrderId);
    Console.WriteLine(cancel.Success
        ? $"Cancelled spot order {order.Data.OrderId}"
        : $"Failed to cancel spot order: {cancel.Error}");
}

static async Task PlaceFuturesReduceOnlyOrderExampleAsync(WhiteBitRestClient client)
{
    Console.WriteLine($"Placing futures V4 reduce-only limit sell order for {futuresSymbol}...");

    var ticker = await GetFuturesTickerAsync(client, futuresSymbol);
    if (ticker == null)
        return;

    var safePrice = Math.Round(ticker.Value * 1.05m, 2);
    var order = await client.V4Api.CollateralTrading.PlaceOrderAsync(
        symbol: futuresSymbol,
        side: OrderSide.Sell,
        type: NewOrderType.Limit,
        quantity: 0.001m,
        price: safePrice,
        reduceOnly: true);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place futures order: {order.Error}");
        return;
    }

    Console.WriteLine($"Placed futures order {order.Data.OrderId}, status: {order.Data.Status}");

    var orderStatus = await client.V4Api.Trading.GetOpenOrdersAsync(futuresSymbol, order.Data.OrderId);
    if (orderStatus.Success)
    {
        var openOrder = orderStatus.Data.SingleOrDefault();
        Console.WriteLine(openOrder == null
            ? "Futures order is no longer open"
            : $"Futures order status: {openOrder.Status}, filled: {openOrder.QuantityFilled}");
    }
    else
    {
        Console.WriteLine($"Failed to query futures order: {orderStatus.Error}");
    }

    var cancel = await client.V4Api.Trading.CancelOrderAsync(futuresSymbol, order.Data.OrderId);
    Console.WriteLine(cancel.Success
        ? $"Cancelled futures order {order.Data.OrderId}"
        : $"Failed to cancel futures order: {cancel.Error}");
}

static async Task<decimal?> GetSpotTickerAsync(WhiteBitRestClient client, string symbol)
{
    var tickers = await client.V4Api.ExchangeData.GetTickersAsync();
    if (!tickers.Success)
    {
        Console.WriteLine($"Failed to get spot ticker: {tickers.Error}");
        return null;
    }

    var ticker = tickers.Data.SingleOrDefault(x => x.Symbol == symbol);
    if (ticker == null)
    {
        Console.WriteLine($"Spot symbol {symbol} not found");
        return null;
    }

    return ticker.LastPrice;
}

static async Task<decimal?> GetFuturesTickerAsync(WhiteBitRestClient client, string symbol)
{
    var symbols = await client.V4Api.ExchangeData.GetFuturesSymbolsAsync();
    if (!symbols.Success)
    {
        Console.WriteLine($"Failed to get futures ticker: {symbols.Error}");
        return null;
    }

    var ticker = symbols.Data.SingleOrDefault(x => x.Symbol == symbol);
    if (ticker == null)
    {
        Console.WriteLine($"Futures symbol {symbol} not found");
        return null;
    }

    return ticker.LastPrice;
}

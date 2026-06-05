
using WhiteBit.Net.Clients;

// REST
var restClient = new WhiteBitRestClient();
var tickers = await restClient.V4Api.ExchangeData.GetTickersAsync();
if (!tickers.Success)
{
    Console.WriteLine($"Failed to get tickers: {tickers.Error}");
    return;
}

var ticker = tickers.Data.SingleOrDefault(x => x.Symbol == "ETH_USDT");
if (ticker == null)
{
    Console.WriteLine("Symbol ETH_USDT not found");
    return;
}

Console.WriteLine($"Rest client ticker price for ETH_USDT: {ticker.LastPrice}");

Console.WriteLine();
Console.WriteLine("Press enter to start websocket subscription");
Console.ReadLine();

// Websocket
var socketClient = new WhiteBitSocketClient();
var subscription = await socketClient.V4Api.SubscribeToTickerUpdatesAsync("ETH_USDT", update =>
{
    Console.WriteLine($"Websocket client ticker price for ETH_USDT: {update.Data.Ticker.LastPrice}");
});

if (!subscription.Success)
{
    Console.WriteLine($"Failed to subscribe to ticker updates: {subscription.Error}");
    return;
}

Console.ReadLine();

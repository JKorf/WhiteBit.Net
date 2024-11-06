
using WhiteBit.Net.Clients;

// REST
var restClient = new WhiteBitRestClient();
var tickers = await restClient.V4Api.ExchangeData.GetTickersAsync();
var ticker = tickers.Data.Single(x => x.Symbol == "ETH_USDT");
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

Console.ReadLine();

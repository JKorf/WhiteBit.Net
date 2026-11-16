// Independent Shared API V2 program. Install WhiteBit.Net.
using CryptoExchange.Net.SharedApis;
using WhiteBit.Net.Clients;

using var rest = new WhiteBitRestClient();
IGetTickerRest ticker = rest.V4Api.SharedApi;
IGetOrderBookRest books = rest.V4Api.SharedApi;
var symbol = new SharedSymbol(TradingMode.Spot, "ETH", "USDT");
await PrintTickerAsync(ticker, symbol);
var book = await books.GetOrderBookAsync(new GetOrderBookRequest(symbol));
if (!book.Success)
    Console.WriteLine(book.Error);
else
    Console.WriteLine($"{books.Exchange}: order book received");

using var socket = new WhiteBitSocketClient();
ISubscribeTickerSocket updates = socket.V4Api.SharedApi;
var subscription = await updates.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(symbol), update => Console.WriteLine(update.Data.LastPrice));
if (!subscription.Success)
{
    Console.WriteLine(subscription.Error);
    return;
}
try
{
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
}
finally
{
    await socket.UnsubscribeAsync(subscription.Data);
}

// Accept another exchange's IGetTickerRest implementation to reuse this function.
static async Task PrintTickerAsync(IGetTickerRest client, SharedSymbol symbol)
{
    var result = await client.GetTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
        Console.WriteLine($"{client.Exchange}: {result.Error}");
    else
        Console.WriteLine($"{client.Exchange}: {result.Data.Symbol} {result.Data.LastPrice}");
}

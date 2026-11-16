// Install WhiteBit.Net. Private streams are enabled only when both credential variables exist.
using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Enums;

using var publicSocket = new WhiteBitSocketClient();
var key = Environment.GetEnvironmentVariable("WHITEBIT_API_KEY");
var secret = Environment.GetEnvironmentVariable("WHITEBIT_API_SECRET");
using var authSocket = new WhiteBitSocketClient(options =>
{
    if (key is not null && secret is not null)
        options.ApiCredentials = new WhiteBitCredentials(key, secret);
});
try
{
    var query = await publicSocket.V4Api.GetTickerAsync("ETH_USDT");
    if (!query.Success)
        Console.WriteLine(query.Error);
    else
        Console.WriteLine(query.Data);
    var ticker = await publicSocket.V4Api.SubscribeToTickerUpdatesAsync(
        "ETH_USDT", update => Console.WriteLine(update.Data.Ticker.LastPrice));
    if (!ticker.Success)
    {
        Console.WriteLine(ticker.Error);
        return;
    }
    var klines = await publicSocket.V4Api.SubscribeToKlineUpdatesAsync(
        "ETH_USDT", KlineInterval.OneMinute, update =>
        {
            foreach (var kline in update.Data)
                Console.WriteLine(kline.ClosePrice);
        });
    if (!klines.Success)
    {
        Console.WriteLine(klines.Error);
        return;
    }
    if (key is not null && secret is not null)
    {
        var orders = await authSocket.V4Api.SubscribeToOpenOrderUpdatesAsync(
            new[] { "ETH_USDT" }, update => Console.WriteLine(update.Data.Order.Status));
        if (!orders.Success)
        {
            Console.WriteLine(orders.Error);
            return;
        }
        var balances = await authSocket.V4Api.SubscribeToSpotBalanceUpdatesAsync(
            new[] { "ETH", "USDT" }, update => Console.WriteLine($"Updated balances: {update.Data.Count}"));
        if (!balances.Success)
        {
            Console.WriteLine(balances.Error);
            return;
        }
    }
    Console.WriteLine("Press Enter to unsubscribe");
    Console.ReadLine();
}
finally
{
    // Both clients belong exclusively to this example, including any partially completed setup.
    await Task.WhenAll(publicSocket.UnsubscribeAllAsync(), authSocket.UnsubscribeAllAsync());
}

using WhiteBit.Net;
using WhiteBit.Net.Interfaces.Clients;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the WhiteBit services
builder.Services.AddWhiteBit();

// OR to provide API credentials for accessing private endpoints, or setting other options:
/*
builder.Services.AddWhiteBit(options =>
{    
   options.ApiCredentials = new WhiteBitCredentials()
        .WithHMAC("<APIKEY>", "<APISECRET>");
   options.Rest.RequestTimeout = TimeSpan.FromSeconds(5);
});
*/

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Map the endpoint and inject the rest client
app.MapGet("/{Symbol}", async ([FromServices] IWhiteBitRestClient client, string symbol) =>
{
    var tickers = await client.V4Api.ExchangeData.GetTickersAsync();
    if (!tickers.Success)
        return Results.Problem(tickers.Error?.Message, statusCode: 502);

    var ticker = tickers.Data.SingleOrDefault(x => x.Symbol == symbol);
    return ticker == null
        ? Results.NotFound($"Symbol {symbol} not found")
        : Results.Ok(ticker.LastPrice);
})
.WithOpenApi();


app.MapGet("/Balances", async ([FromServices] IWhiteBitRestClient client) =>
{
    var result = await client.V4Api.Account.GetSpotBalancesAsync();
    return result.Success
        ? Results.Ok(result.Data)
        : Results.Problem(result.Error?.Message, statusCode: 502);
})
.WithOpenApi();

app.Run();

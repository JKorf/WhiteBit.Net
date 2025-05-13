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
   options.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
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
    var ticker = tickers.Data.Single(x => x.Symbol == symbol);
    return ticker.LastPrice;
})
.WithOpenApi();


app.MapGet("/Balances", async ([FromServices] IWhiteBitRestClient client) =>
{
    var result = await client.V4Api.Account.GetSpotBalancesAsync();
    return (object)(result.Success ? result.Data : result.Error!);
})
.WithOpenApi();

app.Run();
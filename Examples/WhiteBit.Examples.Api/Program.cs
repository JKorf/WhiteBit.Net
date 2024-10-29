using WhiteBit.Net.Interfaces.Clients;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the WhiteBit services
builder.Services.AddWhiteBit();

// OR to provide API credentials for accessing private endpoints, or setting other options:
/*
builder.Services.AddWhiteBit(restOptions =>
{
    restOptions.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
    restOptions.RequestTimeout = TimeSpan.FromSeconds(5);
}, socketOptions =>
{
    socketOptions.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
});
*/

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Map the endpoint and inject the rest client
app.MapGet("/{Symbol}", async ([FromServices] IWhiteBitRestClient client, string symbol) =>
{
    var result = await client.SpotApi.ExchangeData.GetTickerAsync(symbol);
    return result.Data.LastPrice;
})
.WithOpenApi();


app.MapGet("/Balances", async ([FromServices] IWhiteBitRestClient client) =>
{
    var result = await client.SpotApi.Account.GetBalancesAsync();
    return (object)(result.Success ? result.Data : result.Error!);
})
.WithOpenApi();

app.Run();
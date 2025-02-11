# ![WhiteBit.Net](https://raw.githubusercontent.com/JKorf/WhiteBit.Net/main/WhiteBit.Net/Icon/icon.png) WhiteBit.Net  

[![.NET](https://img.shields.io/github/actions/workflow/status/JKorf/WhiteBit.Net/dotnet.yml?style=for-the-badge)](https://github.com/JKorf/WhiteBit.Net/actions/workflows/dotnet.yml) ![License](https://img.shields.io/github/license/JKorf/WhiteBit.Net?style=for-the-badge)

WhiteBit.Net is a client library for accessing the [WhiteBit REST and Websocket API](https://docs.whitebit.com/). 

## Features
* Response data is mapped to descriptive models
* Input parameters and response values are mapped to discriptive enum values where possible
* Automatic websocket (re)connection management 
* Client side rate limiting 
* Client side order book implementation
* Extensive logging
* Support for different environments
* Easy integration with other exchange client based on the CryptoExchange.Net base library

## Supported Frameworks
The library is targeting both `.NET Standard 2.0` and `.NET Standard 2.1` for optimal compatibility

|.NET implementation|Version Support|
|--|--|
|.NET Core|`2.0` and higher|
|.NET Framework|`4.6.1` and higher|
|Mono|`5.4` and higher|
|Xamarin.iOS|`10.14` and higher|
|Xamarin.Android|`8.0` and higher|
|UWP|`10.0.16299` and higher|
|Unity|`2018.1` and higher|

## Install the library

### NuGet 
[![NuGet version](https://img.shields.io/nuget/v/WhiteBit.net.svg?style=for-the-badge)](https://www.nuget.org/packages/WhiteBit.Net)  [![Nuget downloads](https://img.shields.io/nuget/dt/WhiteBit.Net.svg?style=for-the-badge)](https://www.nuget.org/packages/WhiteBit.Net)

	dotnet add package WhiteBit.Net
	
### GitHub packages
WhiteBit.Net is available on [GitHub packages](https://github.com/JKorf/WhiteBit.Net/pkgs/nuget/WhiteBit.Net). You'll need to add `https://nuget.pkg.github.com/JKorf/index.json` as a NuGet package source.

### Download release
[![GitHub Release](https://img.shields.io/github/v/release/JKorf/WhiteBit.Net?style=for-the-badge&label=GitHub)](https://github.com/JKorf/WhiteBit.Net/releases)

The NuGet package files are added along side the source with the latest GitHub release which can found [here](https://github.com/JKorf/WhiteBit.Net/releases).

## How to use
* REST Endpoints
	```csharp
	// Get the ETH/USDT ticker via rest request
	var restClient = new WhiteBitRestClient();
	var tickerResult = await restClient.V4Api.ExchangeData.GetTickersAsync();
	var symbol = tickerResult.Data.Single(x => x.Symbol == "ETH_USDT");
	var lastPrice = symbol.LastPrice;
	```
* Websocket streams
	```csharp
	// Subscribe to ETH/USDT ticker updates via the websocket API
	var socketClient = new WhiteBitSocketClient();
	var tickerSubscriptionResult = socketClient.V4Api.SubscribeToTickerUpdatesAsync("ETH_USDT", (update) =>
	{
	    var lastPrice = update.Data.Ticker.LastPrice;
	});
	```

For information on the clients, dependency injection, response processing and more see the [documentation](https://jkorf.github.io/CryptoExchange.Net), or have a look at the examples [here](https://github.com/JKorf/WhiteBit.Net/tree/main/Examples) or [here](https://github.com/JKorf/CryptoExchange.Net/tree/master/Examples).

## CryptoExchange.Net
WhiteBit.Net is based on the [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net) base library. Other exchange API implementations based on the CryptoExchange.Net base library are available and follow the same logic.

CryptoExchange.Net also allows for [easy access to different exchange API's](https://jkorf.github.io/CryptoExchange.Net#idocs_shared).

|Exchange|Repository|Nuget|
|--|--|--|
|Binance|[JKorf/Binance.Net](https://github.com/JKorf/Binance.Net)|[![Nuget version](https://img.shields.io/nuget/v/Binance.net.svg?style=flat-square)](https://www.nuget.org/packages/Binance.Net)|
|BingX|[JKorf/BingX.Net](https://github.com/JKorf/BingX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.BingX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.BingX.Net)|
|Bitfinex|[JKorf/Bitfinex.Net](https://github.com/JKorf/Bitfinex.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bitfinex.net.svg?style=flat-square)](https://www.nuget.org/packages/Bitfinex.Net)|
|Bitget|[JKorf/Bitget.Net](https://github.com/JKorf/Bitget.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Bitget.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Bitget.Net)|
|BitMart|[JKorf/BitMart.Net](https://github.com/JKorf/BitMart.Net)|[![Nuget version](https://img.shields.io/nuget/v/BitMart.net.svg?style=flat-square)](https://www.nuget.org/packages/BitMart.Net)|
|BitMEX|[JKorf/BitMEX.Net](https://github.com/JKorf/BitMEX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.BitMEX.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.BitMEX.Net)|
|Bybit|[JKorf/Bybit.Net](https://github.com/JKorf/Bybit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bybit.net.svg?style=flat-square)](https://www.nuget.org/packages/Bybit.Net)|
|Coinbase|[JKorf/Coinbase.Net](https://github.com/JKorf/Coinbase.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Coinbase.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Coinbase.Net)|
|CoinEx|[JKorf/CoinEx.Net](https://github.com/JKorf/CoinEx.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinEx.Net)|
|CoinGecko|[JKorf/CoinGecko.Net](https://github.com/JKorf/CoinGecko.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinGecko.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinGecko.Net)|
|Crypto.com|[JKorf/CryptoCom.Net](https://github.com/JKorf/CryptoCom.Net)|[![Nuget version](https://img.shields.io/nuget/v/CryptoCom.net.svg?style=flat-square)](https://www.nuget.org/packages/CryptoCom.Net)|
|Gate.io|[JKorf/GateIo.Net](https://github.com/JKorf/GateIo.Net)|[![Nuget version](https://img.shields.io/nuget/v/GateIo.net.svg?style=flat-square)](https://www.nuget.org/packages/GateIo.Net)|
|HTX|[JKorf/HTX.Net](https://github.com/JKorf/HTX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.HTX.net.svg?style=flat-square)](https://www.nuget.org/packages/Jkorf.HTX.Net)|
|HyperLiquid|[JKorf/HyperLiquid.Net](https://github.com/JKorf/HyperLiquid.Net)|[![Nuget version](https://img.shields.io/nuget/v/HyperLiquid.Net.svg?style=flat-square)](https://www.nuget.org/packages/HyperLiquid.Net)|
|Kraken|[JKorf/Kraken.Net](https://github.com/JKorf/Kraken.Net)|[![Nuget version](https://img.shields.io/nuget/v/KrakenExchange.net.svg?style=flat-square)](https://www.nuget.org/packages/KrakenExchange.Net)|
|Kucoin|[JKorf/Kucoin.Net](https://github.com/JKorf/Kucoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/Kucoin.net.svg?style=flat-square)](https://www.nuget.org/packages/Kucoin.Net)|
|Mexc|[JKorf/Mexc.Net](https://github.com/JKorf/Mexc.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Mexc.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Mexc.Net)|
|OKX|[JKorf/OKX.Net](https://github.com/JKorf/OKX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.OKX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.OKX.Net)|
|XT|[JKorf/XT.Net](https://github.com/JKorf/XT.Net)|[![Nuget version](https://img.shields.io/nuget/v/XT.net.svg?style=flat-square)](https://www.nuget.org/packages/XT.Net)|

When using multiple of these API's the [CryptoClients.Net](https://github.com/JKorf/CryptoClients.Net) package can be used which combines this and the other packages and allows easy access to all exchange API's.

## Discord
[![Nuget version](https://img.shields.io/discord/847020490588422145?style=for-the-badge)](https://discord.gg/MSpeEtSY8t)  
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). For discussion and/or questions around the CryptoExchange.Net and implementation libraries, feel free to join.

## Supported functionality

### V4 API Public
|API|Supported|Location|
|--|--:|--|
|Rest API|✓|`restClient.V4Api.ExchangeData`|
|Websocket API|✓|`socketClient.V4Api`|

### V4 API Private Rest Main
|API|Supported|Location|
|--|--:|--|
|Codes API|✓|`restClient.V4Api.Codes`|
|Crypto Lending API|X||
|Fees API|✓|`restClient.V4Api.Account`|
|SubAccount API|✓|`restClient.V4Api.SubAccount`|
|Mining Pool API|✓|`restClient.V4Api.Account`|

### V4 API Private Rest Trade
|API|Supported|Location|
|--|--:|--|
|Spot API|✓|`restClient.V4Api.Account` / `restClient.V4Api.Trading`|
|Collateral API|✓|`restClient.V4Api.Account` / `restClient.V4Api.CollateralTrading`|
|Convert API|✓|`restClient.V4Api.Convert`|

### V4 API Private WebSocket
|API|Supported|Location|
|--|--:|--|
|*|✓|`socketClient.V4Api`|

## Support the project
Any support is greatly appreciated.

### Referal
If you do not yet have an account please consider using this referal link to sign up:  
[Link](https://whitebit.com/referral/a8e59b59-186c-4662-824c-3095248e0edf)

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1q277a5n54s2l2mzlu778ef7lpkwhjhyvghuv8qf  
**Eth**:  0xcb1b63aCF9fef2755eBf4a0506250074496Ad5b7   
**USDT (TRX)**  TKigKeJPXZYyMVDgMyXxMf17MWYia92Rjd 

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf). 

## Release notes
* Version 1.4.0 - 11 Feb 2025
    * Updated CryptoExchange.Net to version 8.8.0, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Added support for more SharedKlineInterval values
    * Added setting of DataTime value on websocket DataEvent updates
    * Added SelfTradePreventionMode parameter to REST PlaceOrder endpoints, updated Order response model with StpMode and Status properties
    * Added socketClient.V4Api.SubscribeToAccountMarginPositionEventUpdatesAsync and SubscribeToAccountBorrowEventUpdatesAsync subscriptions
    * Added TpSl property to WhtieBitPosition model, containing TakeProfit/StopLoss order reference info
    * Added Role and FeeAsset properties to socketClient.V4Api.SubscribeToUserTradeUpdatesAsync update model
    * Added restClient.V4Api.Account.GetTradingFeesAsync endpoint
    * Updated KlineInterval.ThreeMinute to KlineInterval.ThreeMinutes
    * Fixed socketClient.V4Api.SubscribeToPositionUpdatesAsync UpdateTime being lower case
    * Fix Mono runtime exception on rest client construction using DI

* Version 1.3.2 - 09 Jan 2025
    * Updated CryptoExchange.Net to version 8.6.1, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Disable ping frames for socket connections as it's not stable

* Version 1.3.1 - 07 Jan 2025
    * Updated CryptoExchange.Net version
    * Added Type property to WhiteBitExchange class

* Version 1.3.0 - 23 Dec 2024
    * Updated CryptoExchange.Net to version 8.5.0, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Added SetOptions methods on Rest and Socket clients
    * Added setting of DefaultProxyCredentials to CredentialCache.DefaultCredentials on the DI http client
    * Improved websocket disconnect detection

* Version 1.2.2 - 20 Dec 2024
    * Fixed deserialization of restClient.V4Api.Trading.GetClosedOrdersAsync without results

* Version 1.2.1 - 03 Dec 2024
    * Updated CryptoExchange.Net to version 8.4.3, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Fixed orderbook creation via WhiteBitOrderBookFactory

* Version 1.2.0 - 28 Nov 2024
    * Updated CryptoExchange.Net to version 8.4.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.4.0
    * Added GetFeesAsync Shared REST client implementations
    * Updated WhiteBitOptions to LibraryOptions implementation
    * Updated test and analyzer package versions

* Version 1.1.0 - 19 Nov 2024
    * Updated CryptoExchange.Net to version 8.3.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.3.0
    * Added support for loading client settings from IConfiguration
    * Added DI registration method for configuring Rest and Socket options at the same time
    * Added DisplayName and ImageUrl properties to WhiteBitExchange class
    * Updated client constructors to accept IOptions from DI
    * Removed redundant WhiteBitSocketClient constructor

* Version 1.0.0 - 06 Nov 2024
    * Initial Release


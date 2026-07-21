# WhiteBit.Net AI API Quick Map

Use this file to route common user intents to the correct WhiteBit.Net client member. If a method name or parameter is not listed here, inspect `WhiteBit.Net/Interfaces/Clients/**` before generating code.

## Client Roots

| Intent | Use |
|---|---|
| REST calls | `new WhiteBitRestClient()` |
| WebSocket streams and socket API requests | `new WhiteBitSocketClient()` |
| API key authentication | `options.ApiCredentials = new WhiteBitCredentials("key", "secret")` |
| Live environment | `WhiteBitEnvironment.Live` |
| Custom environment | `WhiteBitEnvironment.CreateCustom(name, restAddress, socketAddress)` |
| Dependency injection | `services.AddWhiteBit(options => { ... })` |
| Direct API root | `client.V4Api` |
| Shared REST client | `new WhiteBitRestClient().V4Api.SharedClient` |
| Shared socket client | `new WhiteBitSocketClient().V4Api.SharedClient` |

## V4 REST: Exchange Data

| User intent | WhiteBit.Net member |
|---|---|
| Get server time | `client.V4Api.ExchangeData.GetServerTimeAsync()` |
| Get symbols / markets | `client.V4Api.ExchangeData.GetSymbolsAsync()` |
| Get system status | `client.V4Api.ExchangeData.GetSystemStatusAsync()` |
| Get all tickers | `client.V4Api.ExchangeData.GetTickersAsync()` |
| Get one ticker | `client.V4Api.ExchangeData.GetTickersAsync()` then filter by `Symbol` |
| Get assets | `client.V4Api.ExchangeData.GetAssetsAsync()` |
| Get order book | `client.V4Api.ExchangeData.GetOrderBookAsync("ETH_USDT")` |
| Get recent trades | `client.V4Api.ExchangeData.GetRecentTradesAsync("ETH_USDT")` |
| Get public deposit/withdrawal info | `client.V4Api.ExchangeData.GetDepositWithdrawalInfoAsync()` |
| Get collateral symbols | `client.V4Api.ExchangeData.GetCollateralSymbolsAsync()` |
| Get futures symbols | `client.V4Api.ExchangeData.GetFuturesSymbolsAsync()` |
| Get public funding history | `client.V4Api.ExchangeData.GetFundingHistoryAsync("BTC_PERP")` |

## V4 REST: Account

| User intent | WhiteBit.Net member |
|---|---|
| Get main balances | `client.V4Api.Account.GetMainBalancesAsync()` |
| Get spot balances | `client.V4Api.Account.GetSpotBalancesAsync()` |
| Get one spot balance | `client.V4Api.Account.GetSpotBalancesAsync("USDT")` |
| Get deposit address | `client.V4Api.Account.GetDepositAddressAsync("USDT", network)` |
| Create deposit address | `client.V4Api.Account.CreateDepositAddressAsync("USDT", network)` |
| Get fiat deposit URL | `client.V4Api.Account.GetFiatDepositAddressAsync(...)` |
| Withdraw | `client.V4Api.Account.WithdrawAsync(...)` |
| Transfer between account types | `client.V4Api.Account.TransferAsync(fromAccount, toAccount, asset, quantity)` |
| Get deposit/withdrawal history | `client.V4Api.Account.GetDepositWithdrawalHistoryAsync(...)` |
| Get deposit/withdrawal settings | `client.V4Api.Account.GetDepositWithdrawalSettingsAsync()` |
| Get mining reward history | `client.V4Api.Account.GetMiningRewardHistoryAsync(...)` |
| Get collateral balances | `client.V4Api.Account.GetCollateralBalancesAsync()` |
| Get collateral balance summary | `client.V4Api.Account.GetCollateralBalanceSummaryAsync()` |
| Get collateral account summary | `client.V4Api.Account.GetCollateralAccountSummaryAsync()` |
| Get account funding history | `client.V4Api.Account.GetAccountFundingHistoryAsync("BTC_PERP")` |
| Set account leverage | `client.V4Api.Account.SetAccountLeverageAsync(leverage)` |
| Get trading fees | `client.V4Api.Account.GetTradingFeesAsync()` |
| Get hedge mode | `client.V4Api.Account.GetHedgeModeAsync()` |
| Set hedge mode | `client.V4Api.Account.SetHedgeModeAsync(enableHedgeMode)` |

## V4 REST: Spot Trading

| User intent | WhiteBit.Net member |
|---|---|
| Place spot order | `client.V4Api.Trading.PlaceSpotOrderAsync(...)` |
| Place spot market buy by quote amount | `client.V4Api.Trading.PlaceSpotOrderAsync(..., type: NewOrderType.Market, quoteQuantity: amount)` |
| Place multiple spot limit orders | `client.V4Api.Trading.PlaceSpotMultipleOrdersAsync(requests)` |
| Cancel spot order | `client.V4Api.Trading.CancelOrderAsync(symbol, orderId)` |
| Cancel spot order by client id | `client.V4Api.Trading.CancelOrderAsync(symbol, clientOrderId: id)` |
| Cancel multiple spot orders | `client.V4Api.Trading.CancelOrdersAsync(requests)` |
| Cancel all spot orders | `client.V4Api.Trading.CancelAllOrdersAsync(symbol)` |
| Get open spot orders | `client.V4Api.Trading.GetOpenOrdersAsync(symbol)` |
| Get closed spot orders | `client.V4Api.Trading.GetClosedOrdersAsync(symbol)` |
| Get spot user trades | `client.V4Api.Trading.GetUserTradesAsync(symbol)` |
| Get trades for an order | `client.V4Api.Trading.GetOrderTradesAsync(orderId)` |
| Edit spot order | `client.V4Api.Trading.EditOrderAsync(symbol, orderId, ...)` |
| Set kill switch | `client.V4Api.Trading.SetKillSwitchAsync(symbol, timeout)` |
| Get kill switch status | `client.V4Api.Trading.GetKillSwitchStatusAsync(symbol)` |

## V4 REST: Collateral / Futures Trading

| User intent | WhiteBit.Net member |
|---|---|
| Place collateral/futures order | `client.V4Api.CollateralTrading.PlaceOrderAsync(...)` |
| Place collateral/futures reduce-only order | `client.V4Api.CollateralTrading.PlaceOrderAsync(..., reduceOnly: true)` |
| Place hedge-mode long/short order | `client.V4Api.CollateralTrading.PlaceOrderAsync(..., positionSide: PositionSide.Long)` |
| Get open positions | `client.V4Api.CollateralTrading.GetOpenPositionsAsync()` |
| Get open positions by symbol | `client.V4Api.CollateralTrading.GetOpenPositionsAsync("ETH_PERP")` |
| Get position history | `client.V4Api.CollateralTrading.GetPositionHistoryAsync(symbol)` |
| Get open conditional orders | `client.V4Api.CollateralTrading.GetOpenConditionalOrdersAsync(symbol)` |
| Place collateral OCO order | `client.V4Api.CollateralTrading.PlaceOcoOrderAsync(...)` |
| Cancel collateral OCO order | `client.V4Api.CollateralTrading.CancelOcoOrderAsync(symbol, orderId)` |
| Cancel conditional order | `client.V4Api.CollateralTrading.CancelConditionalOrderAsync(symbol, orderId)` |
| Cancel OTO order | `client.V4Api.CollateralTrading.CancelOTOOrderAsync(symbol, orderId)` |

## V4 REST: Convert, Codes, Sub-Accounts

| User intent | WhiteBit.Net member |
|---|---|
| Get convert estimate | `client.V4Api.Convert.GetConvertEstimateAsync(fromAsset, toAsset, quantity, direction)` |
| Confirm convert estimate | `client.V4Api.Convert.ConfirmConvertAsync(estimateId)` |
| Get convert history | `client.V4Api.Convert.GetConvertHistoryAsync(...)` |
| Create WhiteBit Code | `client.V4Api.Codes.CreateCodeAsync(asset, quantity, passphrase, description)` |
| Apply WhiteBit Code | `client.V4Api.Codes.ApplyCodeAsync(code, passphrase)` |
| Get created codes | `client.V4Api.Codes.GetCreatedCodesAsync()` |
| Get code history | `client.V4Api.Codes.GetCodeHistoryAsync()` |
| Create sub-account | `client.V4Api.SubAccount.CreateSubAccountAsync(alias, ...)` |
| Delete sub-account | `client.V4Api.SubAccount.DeleteSubAccountAsync(id)` |
| Edit sub-account | `client.V4Api.SubAccount.EditSubAccountAsync(id, alias, spotEnabled, collateralEnabled)` |
| Get sub-accounts | `client.V4Api.SubAccount.GetSubAccountsAsync()` |
| Transfer to/from sub-account | `client.V4Api.SubAccount.SubaccountTransferAsync(id, direction, asset, quantity)` |
| Block sub-account | `client.V4Api.SubAccount.BlockSubaccountAsync(id)` |
| Unblock sub-account | `client.V4Api.SubAccount.UnblockSubaccountAsync(id)` |
| Get sub-account balances | `client.V4Api.SubAccount.GetSubaccountBalancesAsync(id)` |
| Get sub-account transfer history | `client.V4Api.SubAccount.GetSubaccountTransferHistoryAsync()` |

## V4 WebSocket

| User intent | WhiteBit.Net member |
|---|---|
| Request trade history over socket | `socketClient.V4Api.GetTradeHistoryAsync(symbol, limit)` |
| Subscribe trades | `socketClient.V4Api.SubscribeToTradeUpdatesAsync(symbol, handler)` |
| Subscribe many trade streams | `socketClient.V4Api.SubscribeToTradeUpdatesAsync(symbols, handler)` |
| Request last price | `socketClient.V4Api.GetLastPriceAsync(symbol)` |
| Subscribe last price | `socketClient.V4Api.SubscribeToLastPriceUpdatesAsync(symbol, handler)` |
| Request ticker | `socketClient.V4Api.GetTickerAsync(symbol)` |
| Subscribe ticker | `socketClient.V4Api.SubscribeToTickerUpdatesAsync(symbol, handler)` |
| Subscribe many tickers | `socketClient.V4Api.SubscribeToTickerUpdatesAsync(symbols, handler)` |
| Subscribe book ticker | `socketClient.V4Api.SubscribeToBookTickerUpdatesAsync(symbol, handler)` |
| Subscribe all book tickers | `socketClient.V4Api.SubscribeToBookTickerUpdatesAsync(handler)` |
| Request klines | `socketClient.V4Api.GetKlinesAsync(symbol, interval, startTime, endTime)` |
| Subscribe klines | `socketClient.V4Api.SubscribeToKlineUpdatesAsync(symbol, interval, handler)` |
| Request order book | `socketClient.V4Api.GetOrderBookAsync(symbol, depth)` |
| Subscribe order book | `socketClient.V4Api.SubscribeToOrderBookUpdatesAsync(symbol, depth, handler)` |
| Request spot balances | `socketClient.V4Api.GetSpotBalancesAsync()` |
| Subscribe spot balances | `socketClient.V4Api.SubscribeToSpotBalanceUpdatesAsync(assets, handler)` |
| Request margin balances | `socketClient.V4Api.GetMarginBalancesAsync()` |
| Subscribe margin balances | `socketClient.V4Api.SubscribeToMarginBalanceUpdatesAsync(assets, handler)` |
| Request open orders | `socketClient.V4Api.GetOpenOrdersAsync(symbol)` |
| Subscribe open order updates | `socketClient.V4Api.SubscribeToOpenOrderUpdatesAsync(symbols, onOrderMessage)` |
| Request closed orders | `socketClient.V4Api.GetClosedOrdersAsync(symbol, orderTypes)` |
| Subscribe closed order updates | `socketClient.V4Api.SubscribeToClosedOrderUpdatesAsync(symbols, filter, handler)` |
| Request user trades | `socketClient.V4Api.GetUserTradesAsync(symbol)` |
| Subscribe user trades | `socketClient.V4Api.SubscribeToUserTradeUpdatesAsync(symbols, handler)` |
| Subscribe positions | `socketClient.V4Api.SubscribeToPositionUpdatesAsync(handler)` |
| Subscribe borrows | `socketClient.V4Api.SubscribeToBorrowUpdatesAsync(handler)` |
| Subscribe account borrow events | `socketClient.V4Api.SubscribeToAccountBorrowEventUpdatesAsync(handler)` |
| Subscribe margin position events | `socketClient.V4Api.SubscribeToAccountMarginPositionEventUpdatesAsync(handler)` |

## SharedApis

| User intent | WhiteBit.Net member or interface |
|---|---|
| Shared REST client | `new WhiteBitRestClient().V4Api.SharedClient` |
| Shared socket client | `new WhiteBitSocketClient().V4Api.SharedClient` |
| Shared spot ticker REST | `ISpotTickerRestClient.GetSpotTickerAsync(new GetTickerRequest(symbol))` |
| Shared spot symbol REST | `ISpotSymbolRestClient.GetSpotSymbolsAsync(...)` |
| Filter shared symbols by asset classification | `GetSymbolsRequest` base/quote asset type and subtype fields |
| Read cached shared spot symbol catalog | `ISpotSymbolRestClient.SpotSymbolCatalog` |
| Read cached shared futures symbol catalog | `IFuturesSymbolRestClient.FuturesSymbolCatalog` |
| Shared spot order REST | `ISpotOrderRestClient.PlaceSpotOrderAsync(...)` |
| Shared futures order REST | `IFuturesOrderRestClient.PlaceFuturesOrderAsync(...)` |
| Shared balances REST | `IBalanceRestClient.GetBalancesAsync(...)` |
| Shared fees REST | `IFeeRestClient.GetFeesAsync(...)` |
| Shared ticker socket | `ITickerSocketClient.SubscribeToTickerUpdatesAsync(...)` |
| Shared book ticker socket | `IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(...)` |
| Shared order book socket | `IOrderBookSocketClient.SubscribeToOrderBookUpdatesAsync(...)` |
| Shared user trade socket | `IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(...)` |
| Shared position socket | `IPositionSocketClient.SubscribeToPositionUpdatesAsync(...)` |
| Discover shared capabilities | `client.V4Api.SharedClient.Discover()` |

Shared spot/futures symbol models include `DisplayName` plus base/quote asset type and subtype metadata.

For shared socket subscriptions, keep the concrete socket client and unsubscribe with `await socketClient.UnsubscribeAsync(subscription.Data)`.

## Result Handling

| Situation | Pattern |
|---|---|
| REST success check | Direct and shared REST methods return `HttpResult<T>` / `HttpResult` |
| Socket request success check | WebSocket request/response calls return `QueryResult<T>` |
| Socket subscription success check | Direct and shared subscriptions return `WebSocketResult<UpdateSubscription>` |
| Generic success check | `if (!result.Success) { Console.WriteLine(result.Error); return; }` |
| Read result data | Read `result.Data` only after `result.Success` |
| Retry decision | Retry only when `result.Error?.IsTransient == true` |
| Cancellation | Pass `ct: cancellationToken` |

## Common Routing Pitfalls

| Do not use | Use instead |
|---|---|
| `WhiteBitClient` | `WhiteBitRestClient` |
| `ApiCredentials` | `WhiteBitCredentials` |
| `client.SpotApi` | `client.V4Api` |
| `client.FuturesApi` | `client.V4Api.CollateralTrading` and `client.V4Api.Account` |
| `client.GeneralApi` | `client.V4Api.Account`, `Codes`, or `SubAccount` |
| `ETHUSDT` in direct WhiteBit calls | `ETH_USDT` |
| `ETHUSDT` for WhiteBit perpetual examples | `ETH_PERP` |
| `.Data` without `.Success` check | Check `.Success` first |
| `ITickerSocketClient.UnsubscribeAsync(...)` | Keep the concrete socket client and call `socketClient.UnsubscribeAsync(subscription.Data)` |

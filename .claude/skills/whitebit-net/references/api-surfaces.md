# API surfaces

## Clients and scope

`WhiteBitRestClient` / `IWhiteBitRestClient` expose `V4Api` with `ExchangeData`, `Account`, `Trading`, `CollateralTrading`, `SubAccount`, `Convert`, `Codes`, and `SharedApi`. `WhiteBitSocketClient` / `IWhiteBitSocketClient` expose queries, subscriptions, and `SharedApi` on `V4Api`. There are no `SpotApi` or `FuturesApi` roots.

Private access uses `WhiteBitCredentials(key, secret)`. `WhiteBitEnvironment.Live` is built in; `CreateCustom` supplies custom endpoints without promising a sandbox. Native spot symbols use `ETH_USDT`, perpetuals `ETH_PERP`. Discover market metadata before choosing product, quantity, or price.

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


## Shared API V2

`IWhiteBitSharedApiClient` implements `ISharedApiClientBase`, with `V4Rest` and `V4Socket` exposing these actual contracts:

| Surface type | Supported capabilities |
| --- | --- |
| `IWhiteBitRestClientV4SharedApi` | `IGetSpotSymbolsRest`, `IGetTickerRest`, `IGetAllTickersRest`, `IGetRecentTradesRest`, `IGetOrderBookRest`, `IGetBalancesRest`, `IGetAssetRest`, `IGetAllAssetsRest`, `IGetDepositAddressesRest`, `IGetDepositHistoryRest`, `IGetWithdrawalHistoryRest`, `IWithdrawRest`, `IPlaceSpotOrderRest`, `IGetSpotOrderRest`, `IGetOpenSpotOrdersRest`, `IGetClosedSpotOrdersRest`, `IGetSpotOrderTradesRest`, `IGetSpotUserTradeHistoryRest`, `ICancelSpotOrderRest`, `IGetFuturesSymbolsRest`, `IGetLeverageRest`, `ISetLeverageRest`, `IGetOpenInterestRest`, `IGetPositionHistoryRest`, `IPlaceFuturesOrderRest`, `IGetFuturesOrderRest`, `IGetOpenFuturesOrdersRest`, `IGetClosedFuturesOrdersRest`, `IGetFuturesOrderTradesRest`, `IGetFuturesUserTradeHistoryRest`, `ICancelFuturesOrderRest`, `IGetPositionsRest`, `IGetFeesRest`, `IPlaceSpotTriggerOrderRest`, `IGetSpotTriggerOrderRest`, `ICancelSpotTriggerOrderRest`, `IPlaceFuturesTriggerOrderRest`, `IGetFuturesTriggerOrderRest`, `ICancelFuturesTriggerOrderRest`, `ISetFuturesTpSlRest`, `ICancelFuturesTpSlRest`, `IGetBookTickerRest`, `IGetFundingRateHistoryRest`, `ITransferRest`, `IPlaceMultipleSpotOrdersRest` |
| `IWhiteBitSocketClientV4SharedApi` | `ISubscribeBalancesSocket`, `ISubscribeBookTickerSocket`, `ISubscribeKlinesSocket`, `ISubscribeTickerSocket`, `ISubscribeTradesSocket`, `ISubscribeUserTradesSocket`, `ISubscribeSpotOrdersSocket`, `ISubscribePositionsSocket`, `ISubscribeFuturesOrdersSocket`, `IPlaceSpotOrderSocket`, `IEditSpotOrderSocket`, `IEditSpotOrderByClientOrderIdSocket`, `ICancelSpotOrderSocket`, `ICancelSpotOrderByClientOrderIdSocket`, `IPlaceFuturesOrderSocket`, `IEditFuturesOrderSocket`, `IEditFuturesOrderByClientOrderIdSocket`, `ICancelFuturesOrderSocket`, `ICancelFuturesOrderByClientOrderIdSocket` |

For a known operation, assign the typed `.SharedApi` to the required narrow interface. For dynamic code, resolve `GetCapability(SharedCapabilities.Tickers.GetTicker.Rest, TradingMode.Spot)` or use `GetCapabilities(...)` for all matches. Futures selection uses `TradingMode.PerpetualLinear`. Handle a nullable `SharedCapabilityResolution<T>` and inspect `Capability` and `Options`. `Discover()` supplies summary metadata. `PreferredTransport` affects generic selection; `.Rest` requires REST. A descriptor identifies a contract, not guaranteed support.

Inspect supported trading modes, request/exchange parameter rules, limits, and notes before generating dynamic requests. Native APIs retain detailed convert, codes, subaccount, hedge-mode, OCO/OTO, and kill-switch workflows. V1 `.SharedClient` facades remain for compatibility.

## Symbols, catalogs, and quantities

Portable symbols use `SharedSymbol(TradingMode.Spot, "ETH", "USDT")` or the appropriate futures trading mode, rather than native underscore/perpetual names. Successful shared symbol queries apply `GetSymbolsRequest` filters and refresh `SpotSymbolCatalog` or `FuturesSymbolCatalog` for that API/environment.

Returned symbols include `DisplayName`, asset classifications, `MakerFeePercentage`, `TakerFeePercentage`, `PriceStep`, and `QuantityStep` where available. Futures metadata additionally includes `UpperFundingCap`, `LowerFundingCap`, `MaxLongLeverage`, and `MaxShortLeverage`. Missing metadata does not imply unrestricted support or a listed market.

Shared order requests use `SharedQuantity.Base`, `.Quote`, or `.Contracts` only where the operation supports the unit. Response `SharedOrderQuantity` values have nullable base/quote/contracts quantities; absent units are not zero. Ticker volume uses `Volumes`; book-ticker quantities use `BestBidQuantities`/`BestAskQuantities`. Inspect `SharedOrderBook.QuantityType` before interpreting depth quantities.

Native spot market buys can use `quoteQuantity` to specify quote-asset spend. Spot and collateral placement use `NewOrderType`; collateral orders additionally expose `positionSide` and `reduceOnly`. Do not silently translate quote spend into base quantity or assume spot and collateral account settings are interchangeable.

## Services and factories

`services.AddWhiteBit(...)` registers native clients, supported V2 capabilities, `IWhiteBitSharedApiClient`, `IWhiteBitOrderBookFactory`, `IWhiteBitTrackerFactory`, and `IWhiteBitUserClientProvider`. Select the exchange aggregate when several exchanges are registered. The order-book factory exposes `V4`, `Create(SharedSymbol, ...)`, and `CreateV4(nativeSymbol, ...)`; check startup success and stop/dispose owned books and trackers. Multi-user clients require deliberate account and credential selection.

Checkout-only: [public contracts](../../../../WhiteBit.Net/Interfaces/Clients), [Shared API adapters](../../../../WhiteBit.Net/Clients), and [DI registration](../../../../WhiteBit.Net/ExtensionMethods).

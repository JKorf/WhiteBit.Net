---
name: whitebit-net
description: Build C#/.NET WhiteBit integrations with WhiteBit.Net, including V4 market data, authenticated accounts, spot and collateral orders, WebSocket requests/subscriptions, fund and subaccount workflows, and Shared API V2 capabilities.
---

# WhiteBit.Net

```shell
dotnet add package WhiteBit.Net
```

Use `WhiteBitRestClient` and `WhiteBitSocketClient` from `WhiteBit.Net.Clients`. WhiteBit uses a single `V4Api` root. REST groups include `ExchangeData`, `Account`, `Trading` (spot), `CollateralTrading` (margin/perpetual), `Convert`, `Codes`, and `SubAccount`. Socket queries and subscriptions are on `V4Api`.

Public market data needs no credentials; private access uses `WhiteBitCredentials(key, secret)` from `WhiteBit.Net`. There is no passphrase argument. Native spot symbols use `ETH_USDT`; perpetual examples use `ETH_PERP`. The built-in environment is `WhiteBitEnvironment.Live`; custom endpoints do not imply a supported testnet.

Check `Success` before `Data`. REST returns `HttpResult<T>`/`HttpResult`, socket queries return `QueryResult<T>`, and subscriptions return `WebSocketResult<UpdateSubscription>`. Batch calls can have nested item results. Retain successful subscriptions and unsubscribe before disposing their owning socket client. Reuse clients for their workload and respect DI ownership.

Spot placement is `Trading.PlaceSpotOrderAsync`; collateral placement is `CollateralTrading.PlaceOrderAsync`. Both use `NewOrderType`. Choose quantity units explicitly, and account for hedge mode and reduce-only behavior when handling collateral positions. Read operational considerations before generating account-changing workflows.

For portable code, use narrow Shared API V2 capabilities through `.SharedApi`. `.SharedClient` is V1 compatibility. `IWhiteBitSharedApiClient` exposes `V4Rest` and `V4Socket`; dynamic lookup can return `null`, and support depends on trading mode and parameter options. Keep native and shared models separate. No additional skill is required.

## References

- [API surfaces](references/api-surfaces.md): native routing, special workflows, V2 capabilities, symbol catalogs, quantities, and factories.
- [Usage](references/usage.md): public setup, credentials, account reads, explicit trading functions, socket queries/subscriptions, Shared API, and DI.
- [Operational considerations](references/safety.md): batch outcomes, ambiguous submissions, hedge mode, fund movement, and account administration.

Checkout-only: [repository instructions](../../../AGENTS.md) and [complete examples](../../../Examples/README.md). The references above remain usable when only this packaged skill is available.

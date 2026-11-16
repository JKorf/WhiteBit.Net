# Operational considerations

## Credentials and account identity

Private REST, socket queries, and private subscriptions use a key and secret with the intended permissions. Keep credentials, WhiteBit Codes, withdrawal details, and account identifiers out of source and logs. Multi-user client selection must retain account identity. The built-in live environment is production; custom endpoints are not evidence of sandbox support.

## Spot and collateral execution

Validate product, trading status, minimum quantity/value, increments, fees, and account balance from current metadata. A limit order can execute immediately; a price below a recent ticker is not inherently safe. Market-buy quote spend and base quantity are different units.

Collateral/perpetual and margin orders create leveraged exposure. Account leverage and hedge-mode changes affect account configuration beyond one example order. Supply `PositionSide` when hedge mode requires it. For a supported reduce-only close, select the opposite order side and intended position leg; do not infer side or size solely from a generic symbol. Reconcile current positions before closing them.

Cancellation races with fills. An acknowledgement is not proof of no execution. Cancel-all and kill-switch operations can affect many orders. OCO/OTO, trigger, and TP/SL semantics require the correct product and trigger inputs.

## Batch and ambiguous outcomes

`PlaceSpotMultipleOrdersAsync` returns an outer HTTP result containing per-item `CallResult<WhiteBitOrderResponse>` values; inspect each item's `Success` and `Error`. Multi-cancel results likewise require per-item inspection. Preserve successful items when reporting partial failure.

Timeouts, disconnects, or cancellation after submission can leave an order, withdrawal, transfer, or conversion accepted remotely. Query order/account/history state and use supported client identifiers for correlation before repeating a mutation. A transient error or a client identifier is not a universal idempotency guarantee. Bounded backoff based on `Error.IsTransient` can be appropriate for public reads.

## Funds and administration

Withdrawals, transfers, conversion confirmation, Code creation/application, subaccount changes, and deposit-address creation alter funds or account administration. Estimates, balances, history, and settings are separate reads. Validate asset, network, memo, destination, unique identifiers, amount, fees, beneficiary data, and account direction for the requested workflow. Do not treat conversion estimates as completed conversions or a Code as harmless display text.

## Socket state

A successful subscription acknowledges setup; it does not guarantee uninterrupted current state. Keep callbacks short, handle interruption/reconnection, and reconcile account state when missed updates matter. Socket queries have independent results and do not create subscriptions. Close owned subscriptions before disposing the socket client.

# AI-Friendly Examples

These examples are optimized for AI coding assistants and quick onboarding. Each file is:

- **Compilable**: copy into a console project with `dotnet add package WhiteBit.Net`.
- **Self-contained**: single file, no shared helpers.
- **Commented**: explains why the important calls are shaped this way.
- **Idiomatic**: follows current WhiteBit.Net 3.x patterns.

## Files

| File | What it shows |
|---|---|
| `01-spot-quickstart.cs` | Client setup, public ticker lookup, authenticated spot balances, spot limit order, open order query, cancellation |
| `02-collateral-futures.cs` | Collateral/futures leverage, market order, open position retrieval, reduce-only close |
| `03-websocket.cs` | Ticker updates, kline updates, private order and balance streams, proper teardown |
| `04-multi-exchange.cs` | `CryptoExchange.Net.SharedApis` pattern for exchange-agnostic code |
| `05-error-handling.cs` | `HttpResult` patterns, retry, common validation and routing mistakes |

## Running

```bash
dotnet new console -n MyWhiteBitApp
cd MyWhiteBitApp
dotnet add package WhiteBit.Net
# Copy the example .cs file content into Program.cs
# Replace API_KEY / API_SECRET placeholders with your own for private endpoints
dotnet run
```

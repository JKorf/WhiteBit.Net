# Standalone examples

Each file is an independent top-level console program. Copy one into `Program.cs` at a time.

| File | Purpose |
| --- | --- |
| [01-spot-quickstart.cs](01-spot-quickstart.cs) | Public data, balances, and spot orders |
| [02-collateral-futures.cs](02-collateral-futures.cs) | Collateral configuration, orders, and positions |
| [03-websocket.cs](03-websocket.cs) | Socket query, public streams, optional private streams, and cleanup |
| [04-multi-exchange.cs](04-multi-exchange.cs) | Shared API V2 market data and subscriptions |
| [05-error-handling.cs](05-error-handling.cs) | Result handling and bounded public-read retries |

```shell
dotnet new console -n MyWhiteBitApp
cd MyWhiteBitApp
dotnet add package WhiteBit.Net
# Copy ONE example into Program.cs
dotnet build
```

Compilation performs no exchange operations. Running trading examples with valid credentials changes live account state; these are not sandbox programs. The socket example reads `WHITEBIT_API_KEY` and `WHITEBIT_API_SECRET` for optional private subscriptions. Other authenticated walkthroughs use placeholders that require explicit configuration.

See the [consumer skill](../../.claude/skills/whitebit-net/SKILL.md), [operational considerations](../../.claude/skills/whitebit-net/references/safety.md), and [complete projects](../README.md).

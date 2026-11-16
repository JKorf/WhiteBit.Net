# Examples

| Project | Purpose |
| --- | --- |
| [API](WhiteBit.Examples.Api/WhiteBit.Examples.Api.csproj) | Inject clients into a web API |
| [Console](WhiteBit.Examples.Console/WhiteBit.Examples.Console.csproj) | Basic client use |
| [Order book](WhiteBit.Examples.OrderBook/WhiteBit.Examples.OrderBook.csproj) | Maintained local order book |
| [Tracker](WhiteBit.Examples.Tracker/WhiteBit.Examples.Tracker.csproj) | Trade tracking |
| [V4 order placement](WhiteBit.Examples.V4OrderPlacement/WhiteBit.Examples.V4OrderPlacement.csproj) | Spot and collateral order submissions |
| [Standalone programs](ai-friendly/README.md) | Focused REST, socket, V2, and error-handling examples |

From the repository root, compile with `dotnet build Examples/Examples.sln`. Builds do not execute exchange calls. Running examples can contact live markets or change account state when credentials are configured.

See the [consumer skill](../.claude/skills/whitebit-net/SKILL.md) and [repository instructions](../AGENTS.md).

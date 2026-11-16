# Working on WhiteBit.Net

Consumer guidance belongs to the [WhiteBit.Net skill](.claude/skills/whitebit-net/SKILL.md); [llms.txt](llms.txt) indexes its references. This file describes repository development.

## Layout and implementation

- [Public contracts](WhiteBit.Net/Interfaces/Clients) and [clients](WhiteBit.Net/Clients): V4 REST groups, socket queries/subscriptions, V1 compatibility, and V2 adapters.
- [Models/options](WhiteBit.Net/Objects), [enums](WhiteBit.Net/Enums), and [converters](WhiteBit.Net/Converters): wire models, configuration, enum mappings, and JSON serialization metadata.
- [DI extensions](WhiteBit.Net/ExtensionMethods), [order books](WhiteBit.Net/SymbolOrderBooks), [tests](WhiteBit.Net.UnitTests), and [examples](Examples/README.md).

Follow neighboring implementations for request construction, signing, validation, nullable annotations, cancellation, XML documentation, and result handling. Use `ConfigureAwait(false)` on library awaits as required by the analyzer. Update interfaces with implementations, models with their serialization metadata, and Shared API adapters with accurate capabilities and parameter options. Keep spot orders, collateral orders, socket requests, and subscriptions distinct. Preserve per-item errors in batch results. Update relevant request/response fixtures when endpoint behavior changes.

Preserve public compatibility across all declared target frameworks. Keep unrelated dependencies, release settings, and generated output outside focused changes.

## Build and test

Use the SDK selected by [CI](.github/workflows/dotnet.yml). From the repository root:

```powershell
dotnet restore WhiteBit.Net.sln
dotnet build WhiteBit.Net.sln --no-restore
dotnet test WhiteBit.Net.UnitTests/WhiteBit.Net.UnitTests.csproj --no-build --filter "FullyQualifiedName~AiExampleCompileTests"
```

Debug normally builds only `netstandard2.0` and `net11.0`. Validate every declared framework and the package with:

```powershell
dotnet build WhiteBit.Net/WhiteBit.Net.csproj -c Release -p:BuildAllFrameworks=true
dotnet pack WhiteBit.Net/WhiteBit.Net.csproj -c Release --no-build
```

`Documentation/AiExampleCompileTests.cs` compiles the standalone examples without executing their exchange operations. Run relevant fixture tests for implementation changes. Live REST/socket integration fixtures have their own execution gates; shared REST integration uses `ManualRun` or `INTEGRATION=1`. Do not enable live tests for documentation validation. Build [complete examples](Examples/Examples.sln) without running them; order-placement examples can change live account state when executed.

## Documentation ownership

Keep API routing in `.claude/skills/whitebit-net/references/api-surfaces.md`, focused snippets in `references/usage.md`, and distinct operational considerations in `references/safety.md`. `SKILL.md` routes to those references, `CLAUDE.md` imports this file, and `llms.txt` is a concise link index.

The [project](WhiteBit.Net/WhiteBit.Net.csproj) packs `.claude/skills/**` under `skills/`, without appending `%(RecursiveDir)`. Validate local links/anchors and compile changed snippets/examples. Inspect the archive for exactly five skill files under `skills/whitebit-net/`, matching source bytes. Label repository-source/example links within the skill as checkout-only so packaged guidance remains usable independently.

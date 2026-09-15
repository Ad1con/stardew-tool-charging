# Tool Charging — 1.6 update plan

Verified against the decompiled `Stardew Valley.dll` from the local install (1.6.15 build 24356, SMAPI 4.5.2).
Original mod: mralbobo/stardew-tool-charging, last commit 2018-11-28, targets SMAPI 2.x / game 1.2–1.3. GPL-3.

## How charging works in 1.6.15 (Game1.UpdateControlInput)

Runs every tick while the use-tool button is held, `player.canReleaseTool` is true, no event/dialogue is up,
stamina >= 1, and the tool is not a fishing rod:

```csharp
int reach = tool.hasEnchantmentOfType<ReachingToolEnchantment>() ? 1 : 0;
if (player.toolHold.Value <= 0 && tool.upgradeLevel.Value + reach > player.toolPower.Value)
{
    player.toolHold.Value = (int)(600f * tool.AnimationSpeedModifier);   // start of a new level
    player.toolHoldStartTime.Value = player.toolHold.Value;
}
else if (tool.upgradeLevel.Value + reach > player.toolPower.Value)
{
    player.toolHold.Value -= time.ElapsedGameTime.Milliseconds;           // countdown
    if (player.toolHold.Value <= 0)
        player.toolPowerIncrease();                                        // next level, sound, sprite, jitter
}
```

So: 600 ms per power level, counting *down*. Identical shape to 1.2. What changed:

| 1.2 (old mod)                          | 1.6.15                                                                 |
|----------------------------------------|------------------------------------------------------------------------|
| `Game1.player.toolHold` is an `int`    | `NetInt` — read/write through `.Value`                                 |
| `GameEvents.UpdateTick`                | `helper.Events.GameLoop.UpdateTicked` (SMAPI 3+/4 event API)           |
| .NET Framework 4.5.2, XNA              | .NET 6, MonoGame (handled entirely by ModBuildConfig)                  |
| `Pathoschild.Stardew.ModBuildConfig` 2.2 (packages.config, old-style csproj) | 4.4.0, SDK-style csproj              |
| Ships its own Newtonsoft.Json          | Never ship it; SMAPI provides it                                       |
| No `MinimumApiVersion` in manifest     | Required                                                               |
| —                                      | New `toolHoldStartTime` (used by `canStrafeForToolUse`, harmless)      |
| —                                      | New `Tool.AnimationSpeedModifier` scales the 600 ms (default 1.0)      |

The audio backend, SDL, networking rewrite, split-screen, and Ginger Island changes listed in the brief do not
touch this code path. Split-screen: SMAPI raises `UpdateTicked` per screen and `Game1.player` is per-screen,
so the same code works unchanged.

## Why the old trick still works, and the one trap

The old mod subtracts an extra N ms from `toolHold` each tick, clamping to 1 rather than 0. That clamp is
load-bearing: `toolPowerIncrease()` is only called from the game's own decrement. If the mod drives
`toolHold` to <= 0 itself, the next game tick takes the *first* branch (reset to 600) and the level is
skipped silently. Clamp to 1, let the game finish the last millisecond. Costs at most one frame per level.

No Harmony needed. The whole mod is a ~20-line `UpdateTicked` handler.

## Config

One setting: `ChargeSpeedMultiplier` (float, default 4.0).

- `1.0` = vanilla. `4.0` = each level takes 150 ms instead of 600. `10.0` = 60 ms.
- Per tick the mod removes `(multiplier - 1) * elapsedMs` on top of the game's own `elapsedMs`, with a
  fractional carry so 1.5x is really 1.5x and not rounded to whole milliseconds per tick.
- Floor is 1.0 (vanilla); no slowing down.
- Clamp to [1, 20].
- Register with Generic Mod Config Menu if present (optional dependency, slider 1.0–20.0 step 0.1, plus
  free text via config.json for anything outside that). GMCM 1.16.0 is installed locally.

## Files

```
ToolCharging/
  ToolCharging.csproj      SDK-style, net6.0, ModBuildConfig 4.4.0, GamePath -> F:\...\Stardew Valley
  manifest.json            new UniqueID (Adicon.ToolCharging), MinimumApiVersion 4.0.0, GMCM as optional dep
  ModEntry.cs              Entry, UpdateTicked handler, GMCM registration
  ModConfig.cs             ChargeSpeedMultiplier
  IGenericModConfigMenuApi.cs   the standard copy-paste interface
  i18n/default.json        GMCM labels
README.md                  credit mralbobo, GPL-3 stays
```

Delete: `packages.config`, `app.config`, `Properties/AssemblyInfo.cs`, the `.sln` (a bare csproj builds fine).

## Build and test

1. `dotnet build -c Release` — ModBuildConfig copies the mod into `F:\...\Stardew Valley\Mods\ToolCharging`
   and zips a release into `bin/Release`.
2. Launch via SMAPI, load a save with a copper+ hoe or watering can. Hold left click; count the charge
   sounds. At 2.0x, three levels should take ~0.9 s instead of ~1.8 s.
3. Check `SMAPI-latest.txt` for the GMCM registration line and no warnings.
4. Edge cases to eyeball: releasing mid-level, Reaching enchantment (one extra level), stamina < 1,
   multiplayer farmhand (charge runs locally; `toolHold` sync is the game's job).

## Publishing (later, not part of the build)

Manifest Author "Adicon", UpdateKeys `GitHub:Ad1con/<repo>` (+ Nexus once uploaded). Keep GPL-3 and credit
mralbobo in README and manifest description.

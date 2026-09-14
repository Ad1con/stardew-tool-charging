# Tool Charging

A [Stardew Valley](https://stardewvalley.net/) mod that speeds up charging a hoe or watering can. Upgraded tools
hit more tiles the longer you hold the button; this shortens the hold.

## Credit

**This is [mralbobo's Tool Charging](https://github.com/mralbobo/stardew-tool-charging), updated for Stardew
Valley 1.6.** The idea and the original implementation are theirs; the original was last updated in 2018 for
game version 1.2/1.3 and no longer loads. This version ports it to SMAPI 4 and .NET 6, changes the setting to a
speed multiplier, and adds Generic Mod Config Menu support. The original git history is preserved in this repo,
and the license is unchanged (GPL-3).

## Install

1. Install [SMAPI](https://smapi.io/).
2. Unzip the mod into `Stardew Valley/Mods`.
3. Run the game through SMAPI.

## Config

One setting, editable in-game with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
or in `config.json`:

| Setting | Default | Meaning |
|---|---|---|
| `ChargeSpeedMultiplier` | `2.0` | How much faster than normal a tool charges. `1.0` is the game's normal speed, `2.0` is twice as fast, `10.0` is ten times. Clamped to 1–50. |

## Compatibility

- Stardew Valley 1.6+, SMAPI 4.0+. Works in multiplayer and split-screen.
- No Harmony patches. It adjusts the game's own charge timer each tick, so it should keep working across game
  updates unless the charge mechanic itself changes.

## Build

Needs the .NET 6+ SDK. `dotnet build -c Release` inside `ToolCharging/` compiles the mod, copies it into your
game's `Mods` folder and zips a release into `bin/Release`. If the game isn't found automatically, set `GamePath`
in a `stardewvalley.targets` file in your home folder, per the
[mod build package docs](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md).

## License

GPL-3, same as the original.

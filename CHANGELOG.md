# Changelog

## 2.0.2 — 2026-09-14

- Default `ChargeSpeedMultiplier` is now 4.0 (was 2.0). Existing `config.json` files keep their value.
- Maximum is now 20 (was 50), matching the in-game slider.

## 2.0.1 — 2026-09-14

- Added the Nexus update key. No functional change.

## 2.0.0 — 2026-09-14

- Updated for Stardew Valley 1.6 and SMAPI 4 (.NET 6, `Farmer.toolHold` as a net field, new event API).
- The setting is now `ChargeSpeedMultiplier`, a multiple of the game's normal charge speed, instead of a raw
  millisecond count per tick.
- Generic Mod Config Menu support.
- Split-screen safe.

## 1.2.1 and earlier

mralbobo's original releases. See https://github.com/mralbobo/stardew-tool-charging.

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ToolCharging;

/// <summary>
/// The game charges a tool by counting <see cref="Farmer.toolHold"/> down from 600 ms per power level
/// (Game1.UpdateControlInput). Each tick this mod removes the extra time a faster charge would have
/// removed, so a multiplier of 2 halves the wait. The value is clamped to 1, never 0: the game only calls
/// toolPowerIncrease from its own decrement, and a toolHold it finds already at 0 is treated as a fresh
/// level and reset to 600.
/// </summary>
public sealed class ModEntry : Mod
{
    private ModConfig Config = null!;

    /// <summary>Fractional milliseconds not yet applied, so non-integer multipliers stay exact. Per screen for split-screen.</summary>
    private readonly PerScreen<double> Carry = new();

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<ModConfig>();
        Config.Clamp();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        Farmer player = Game1.player;
        int hold = player.toolHold.Value;
        if (!player.canReleaseTool || hold <= 0)
        {
            Carry.Value = 0;
            return;
        }

        // the game already removed this frame's elapsed ms; remove what the remaining (multiplier - 1) would have
        double extra = (Config.ChargeSpeedMultiplier - 1) * Game1.currentGameTime.ElapsedGameTime.Milliseconds + Carry.Value;
        int take = (int)Math.Truncate(extra);
        Carry.Value = extra - take;
        if (take == 0)
            return;

        player.toolHold.Value = Math.Max(1, hold - take);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (gmcm is null)
            return;

        gmcm.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () =>
            {
                Config.Clamp();
                Helper.WriteConfig(Config);
            }
        );

        gmcm.AddNumberOption(
            mod: ModManifest,
            getValue: () => Config.ChargeSpeedMultiplier,
            setValue: value => Config.ChargeSpeedMultiplier = value,
            name: () => Helper.Translation.Get("config.multiplier.name"),
            tooltip: () => Helper.Translation.Get("config.multiplier.tooltip"),
            min: 1f,
            max: 20f,
            interval: 0.1f,
            formatValue: value => $"{value:0.0}x"
        );
    }
}

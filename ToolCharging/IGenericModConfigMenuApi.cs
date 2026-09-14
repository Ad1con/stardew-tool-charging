using StardewModdingAPI;

namespace ToolCharging;

/// <summary>The subset of the Generic Mod Config Menu API this mod uses. See https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu.</summary>
public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

    void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null);
}

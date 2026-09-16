namespace ToolCharging;

public sealed class ModConfig
{
    /// <summary>How much faster a tool charges compared to vanilla. 1.0 is vanilla, 4.0 is four times as fast.</summary>
    public float ChargeSpeedMultiplier { get; set; } = 4f;

    /// <summary>Skip the charge-up entirely: each power level completes on the next frame. Overrides <see cref="ChargeSpeedMultiplier"/>.</summary>
    public bool InstantCharge { get; set; } = false;

    public const float Min = 1f;
    public const float Max = 20f;

    public void Clamp()
    {
        ChargeSpeedMultiplier = Math.Clamp(ChargeSpeedMultiplier, Min, Max);
    }
}

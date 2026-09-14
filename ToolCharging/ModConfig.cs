namespace ToolCharging;

public sealed class ModConfig
{
    /// <summary>How much faster a tool charges compared to vanilla. 1.0 is vanilla, 2.0 is twice as fast.</summary>
    public float ChargeSpeedMultiplier { get; set; } = 2f;

    public const float Min = 1f;
    public const float Max = 50f;

    public void Clamp()
    {
        ChargeSpeedMultiplier = Math.Clamp(ChargeSpeedMultiplier, Min, Max);
    }
}

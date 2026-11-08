namespace MiniGolf.Misc;

public static class Settings
{
    public enum WindStrength
    {
        VeryWeak,
        Weak,
        Strong,
        VeryStrong,
    }

    public static bool SfxEnabled { get; set; }

    public static bool BGMEnabled { get; set; }

    public static float GetWindStrength(WindStrength strength)
    {
        return strength switch
        {
            WindStrength.VeryWeak => 0.05f,
            WindStrength.Weak => 0.10f,
            WindStrength.Strong => 0.25f,
            WindStrength.VeryStrong => 0.5f,
            _ => 0.00f,
        };
    }
}

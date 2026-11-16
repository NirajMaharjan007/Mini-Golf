namespace MiniGolf.Misc;

public static class Settings
{
    public enum WindStrength
    {
        None,
        VeryWeak,
        Weak,
        Strong,
        VeryStrong,
    }

    public enum WindDirection
    {
        Left,
        Right,
    }

    // only for rich Text Label
    public const string RightArrow = "[b]\U0001F87A[/b]",
        LeftArrow = "[b]\U0001F878[/b]",
        Cross = "[b]\U0001F7AE[/b]";

    public static bool SfxEnabled { get; set; }

    public static bool BGMEnabled { get; set; }

    public static float GetWindStrength(WindStrength strength)
    {
        return strength switch
        {
            WindStrength.None => 0.00f,
            WindStrength.VeryWeak => (float)Godot.GD.RandRange(0.05f, 0.09f),
            WindStrength.Weak => (float)Godot.GD.RandRange(0.10f, 0.24f),
            WindStrength.Strong => (float)Godot.GD.RandRange(0.25f, 0.45f),
            WindStrength.VeryStrong => (float)Godot.GD.RandRange(0.5f, 1.20f),
            _ => 0.00f,
        };
    }

    public static int GetWindDirection(WindDirection direction)
    {
        return direction switch
        {
            WindDirection.Left => -1,
            WindDirection.Right => 1,
            _ => 0,
        };
    }

    public static float CurrentWindDirection { set; get; }

    public static float CurrentWindStrength { set; get; }
}

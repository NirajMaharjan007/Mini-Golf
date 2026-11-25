using Godot;

namespace MiniGolf.Misc;

public static class Configuration
{
    private static readonly ConfigFile config = new();

    public static void Load()
    {
        var check = config.Load("user://settings.cfg");

        if (check.Equals(Error.FileNotFound))
        {
            // Store some values (Section, Key, Value)
            config.SetValue("Audio", "SFX", true);
            config.SetValue("Audio", "BGM", true);
            config.SetValue("Gameplay", "Wind_Strength", 0);
        } // If the file exists, config already contains the old data and can be modified directly
        else if (!check.Equals(Error.Ok))
        {
            // Other read errors (such as corruption), handle as needed
            GD.PushError("Failed to load settings: " + check);
            return;
        }

        // Save to the user directory (persists outside the project folder)
        var err = config.Save("user://settings.cfg");

        if (!err.Equals(Error.Ok))
        {
            GD.PushError($"Failed to save settings: {err}");
        }
    }

    public static string DebugValueString
    {
        get
        {
            var check = config.Load("user://settings.cfg");
            if (!check.Equals(Error.FileNotFound))
            {
                var bgm = config.GetValue("Audio", "BGM");
                var sfx = config.GetValue("Audio", "SFX");
                var windStrength = config.GetValue("Gameplay", "Wind_Strength");

                return $"BGM {bgm} SFX {sfx} WindStrength {windStrength}";
            }

            return "BLANK";
        }
    }
}

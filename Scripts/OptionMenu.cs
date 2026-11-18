using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class OptionMenu : Control
{
    private bool sfxEnable,
        bgmEnabled;

    private OptionButton windStrength;

    private CheckButton sfxBtn,
        bgmBtn;

    private Button saveBtn,
        backBtn;

    private Label sfxLabel,
        bgmLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var optionBtnContainer = GetNode<HBoxContainer>(
            "PanelContainer/CenterContainer/MainContainer/BtnContainer"
        );

        saveBtn = optionBtnContainer.GetNode<Button>("Save");

        backBtn = optionBtnContainer.GetNode<Button>("Back");
        backBtn.Pressed += HandleBack;

        windStrength = GetNode<OptionButton>(
            "PanelContainer/CenterContainer/MainContainer/WindContainer/OptionButton"
        );

        var addedLabels = new System.Collections.Generic.HashSet<string>();

        foreach (
            Settings.WindStrength strength in System.Enum.GetValues(typeof(Settings.WindStrength))
        )
        {
            // Use your existing switch statement to get the label
            string label = Settings.GetWindStrengthText(strength);

            if (!addedLabels.Add(label)) // Add returns false if it already exists
                continue;

            // Add item: Label on screen, Enum int value as the ID
            windStrength.AddItem(label, (int)strength);
        }

        var sfxContainer = GetNode<HBoxContainer>(
            "PanelContainer/CenterContainer/MainContainer/SFXContainer"
        );

        sfxLabel = sfxContainer.GetNode<Label>("BtnContainer/Label");
        // sfxLabel.Text = "WOW";
        sfxBtn = sfxContainer.GetNode<CheckButton>("BtnContainer/CheckButton");
        sfxBtn.Pressed += () =>
        {
            sfxLabel.Text = sfxBtn.ButtonPressed ? "YES" : "NO";
            sfxEnable = sfxBtn.ButtonPressed is true; //Strongly Aggreed
        };

        var bgmContainer = GetNode<HBoxContainer>(
            "PanelContainer/CenterContainer/MainContainer/BGMContainer"
        );

        bgmLabel = bgmContainer.GetNode<Label>("BtnContainer/Label");
        // bgmLabel.Text = "Wow";
        bgmBtn = bgmContainer.GetNode<CheckButton>("BtnContainer/CheckButton");
        bgmBtn.Pressed += () =>
        {
            bgmLabel.Text = bgmBtn.ButtonPressed ? "YES" : "NO";
            bgmEnabled = bgmBtn.ButtonPressed is true; //Strongly Aggreed
        };

        windStrength.Selected = 0;
        windStrength.ItemSelected += OnWindSelection;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private async void HandleBack()
    {
        await SceneManager.ChangeScene("res://Scenes/Menu/Menu.tscn");
    }

    private void OnWindSelection(long index)
    {
        Print($"index {index} + item-name {windStrength.GetItemText((int)index)}");
    }
}

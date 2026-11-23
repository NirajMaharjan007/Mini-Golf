using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Menu : Control
{
    private Button startBtn,
        optionBtn,
        creditBtn,
        exitBtn;

    private AudioStreamPlayer clickSFX,
        backSFX;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        clickSFX = GetNode<AudioStreamPlayer>("Click");
        backSFX = GetNode<AudioStreamPlayer>("Back");

        var btnContainer = GetNode<VBoxContainer>("Panel/PanelContainer/ButtonContainer");

        startBtn = btnContainer.GetNode<Button>("Start");
        startBtn.Pressed += HandleChanges;

        optionBtn = btnContainer.GetNode<Button>("Option");
        optionBtn.Pressed += HandleOption;

        creditBtn = btnContainer.GetNode<Button>("Credit");

        exitBtn = btnContainer.GetNode<Button>("Exit");
        exitBtn.Pressed += HandleExit;
    }

    private async void HandleExit()
    {
        backSFX.Play();
        await SceneManager.ExitScene();
    }

    private async void HandleChanges()
    {
        try
        {
            clickSFX.Play();
            SceneManager.NextScene = "res://Scenes/Game/Main.tscn";
            await SceneManager.LoadingScene();
        }
        catch (System.Exception e)
        {
            PrintErr("ERROR: " + e.Message);
        }
    }

    private async void HandleOption()
    {
        clickSFX.Play();
        await SceneManager.ChangeScene("res://Scenes/Menu/OptionMenu.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}

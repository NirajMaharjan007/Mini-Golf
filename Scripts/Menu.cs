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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var btnContainer = GetNode<VBoxContainer>("PanelContainer/CenterContainer/ButtonContainer");

        startBtn = btnContainer.GetNode<Button>("Start");
        startBtn.Pressed += HandleChanges;

        optionBtn = btnContainer.GetNode<Button>("Option");
        creditBtn = btnContainer.GetNode<Button>("Credit");

        exitBtn = btnContainer.GetNode<Button>("Exit");
        exitBtn.Pressed += HandleExit;
    }

    private async void HandleExit()
    {
        await SceneManager.ExitScene();
    }

    private async void HandleChanges()
    {
        try
        {
            SceneManager.NextScene = "res://Scenes/Game/Main.tscn";
            await SceneManager.LoadingScene();
        }
        catch (System.Exception e)
        {
            PrintErr("ERROR: " + e.Message);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}

using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Pause : Panel
{
    [Signal]
    public delegate void ResumeRequestedEventHandler();

    private Button resumeBtn,
        backBtn,
        exitBtn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var btnContainer = GetNode<VBoxContainer>("Outerpanel/InnerContainer/BtnContainer");

        resumeBtn = btnContainer.GetNode<Button>("Resume");
        resumeBtn.Pressed += HandleResume;

        backBtn = btnContainer.GetNode<Button>("Back");
        backBtn.Pressed += HandleMainMenu;

        exitBtn = btnContainer.GetNode<Button>("Exit");
        exitBtn.Pressed += HandleExit;
    }

    private async void HandleMainMenu()
    {
        try
        {
            SceneManager.NextScene = "res://Scenes/Menu/Menu.tscn";
            await SceneManager.LoadingScene();
        }
        catch (System.Exception ex)
        {
            PrintErr($"ERROR: {ex.Message}\nCause: {ex.Source}");
            System.Environment.Exit(1);
        }
    }

    private void HandleResume()
    {
        EmitSignal(SignalName.ResumeRequested);
    }

    private async void HandleExit()
    {
        await SceneManager.ExitScene();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public override void _ExitTree()
    {
        base._ExitTree();

        try
        {
            resumeBtn.Pressed -= HandleResume;
            backBtn.Pressed -= HandleMainMenu;
            exitBtn.Pressed -= HandleExit;
        }
        catch (System.Exception ex)
        {
            PrintErr("ERROR: " + ex.Message);
        }
    }
}

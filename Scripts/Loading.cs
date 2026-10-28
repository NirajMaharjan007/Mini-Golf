using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Loading : Control
{
    private ProgressBar progressBar;

    private string _nextPath;

    private bool _switching;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer");

        progressBar = container.GetNode<ProgressBar>("ProgressBar");
        progressBar.Value = 0;

        _nextPath = string.IsNullOrEmpty(SceneManager.NextScene)
            ? "res://Scenes/Menu/Menu.tscn"
            : SceneManager.NextScene;

        // Kick off the threaded load ONCE
        var err = ResourceLoader.LoadThreadedRequest(_nextPath);
        if (err != Error.Ok)
        {
            PrintErr($"Loading: failed to request {_nextPath}: {err}");
            return;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
    {
        if (_switching)
            return;

        var randomize = (float)RandRange(0.08, 0.24);

        Print($"{randomize} {_nextPath}");

        if (progressBar.Value < 100)
        {
            progressBar.Value += randomize;
        }
        else
        {
            _switching = true;
            await SceneManager.ChangeScene(_nextPath);
        }
    }
}

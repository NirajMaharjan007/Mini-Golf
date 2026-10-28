using Godot;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Loading : Control
{
    private ProgressBar progressBar;

    private string _loadingPath;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer");

        progressBar = container.GetNode<ProgressBar>("ProgressBar");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // var status = ResourceLoader.LoadThreadedGetStatus(_loadingPath, progress);
        // var progress = new Godot.Collections.Array();

        progressBar.Value += 0.05f;

        Print($"{progressBar.Value}");
    }
}

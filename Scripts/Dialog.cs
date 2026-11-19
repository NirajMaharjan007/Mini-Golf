using Godot;

namespace MiniGolf.Scripts;

public partial class Dialog : Panel
{
    [Signal]
    public delegate void BackRequestEventHandler();

    private Button backBtn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var container = GetNode<HBoxContainer>("Outerpanel/InnerContainer/ConfirmCotainer");

        backBtn = container.GetNode<Button>("No");
        backBtn.Pressed += HandleBack;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void HandleBack()
    {
        EmitSignal(SignalName.BackRequest);
    }
}

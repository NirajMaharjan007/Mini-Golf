using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Main : Node2D
{
    private Camera2D camera;

    private Ball ball;

    private Label count;

    // private Ball

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ball = GetNode<Ball>("Ball");

        count = GetNode<Label>("CanvasLayer/Panel/HBoxContainer/Number");
        count.Text = ball.Counter.ToString();

        camera = GetNode<Camera2D>("Ball/Camera2D");

        // camera.LimitRight = GetWindow().Size.X;
        camera.LimitBottom = GetWindow().Size.Y;

        Print("this is a limit " + camera.LimitBottom);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        count.Text = ball.Counter.ToString();
    }
}

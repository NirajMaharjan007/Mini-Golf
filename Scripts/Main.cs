using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Main : Node2D
{
    private Camera2D camera;

    private Ball ball;

    private Label count;

    private Button pauseBtn;

    private CanvasLayer pauseCanvas;

    private Pause pause;

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

        pauseCanvas = GetNode<CanvasLayer>("PauseCanvas");

        pause = pauseCanvas.GetNode<Pause>("Pause");
        pause.ResumeRequested += HandlePause;

        pauseBtn = GetNode<Button>("CanvasLayer/HBoxContainer/Pause");
        pauseBtn.Pressed += HandlePause;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed && !keyEvent.Echo)
            {
                if (keyEvent.Keycode == Key.Escape)
                {
                    Print("Escape pressed!");

                    HandlePause();
                }
            }
        }
    }

    private void HandlePause()
    {
        Print("Clicked" + pauseCanvas.Visible);
        pauseCanvas.Visible = !pauseCanvas.Visible;
        GetTree().Paused = !GetTree().Paused;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        count.Text = ball.Counter.ToString();
    }
}

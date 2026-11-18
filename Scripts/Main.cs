using Godot;
using MiniGolf.Misc;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Main : Node2D
{
    private Camera2D camera;

    private Ball ball;

    private Label count,
        windLabel;

    private RichTextLabel windDirectionLabel;

    private Button pauseBtn,
        resetBtn;

    private CanvasLayer pauseCanvas;

    private Pause pause;

    private AudioStreamPlayer2D windSFX;

    private bool _windApplied = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        ball = GetNode<Ball>("Ball");
        ball.IsStop = true;

        count = GetNode<Label>("CanvasLayer/Main/Panel/HBoxContainer/Number");
        count.Text = ball.Counter.ToString();

        camera = GetNode<Camera2D>("Ball/Camera2D");

        // camera.LimitRight = GetWindow().Size.X;
        camera.LimitBottom = GetWindow().Size.Y;

        Print("this is a limit " + camera.LimitBottom);

        pauseCanvas = GetNode<CanvasLayer>("PauseCanvas");

        pause = pauseCanvas.GetNode<Pause>("Pause");
        pause.ResumeRequested += HandlePause;

        pauseBtn = GetNode<Button>("CanvasLayer/Main/HBoxContainer/Pause");
        pauseBtn.Pressed += HandlePause;

        resetBtn = GetNode<Button>("CanvasLayer/Main/HBoxContainer/Reset");
        resetBtn.Pressed += HandleReset;
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

    private static float GetRandomWindStrength(Settings.WindStrength strength)
    {
        var result = System.Math.Round(Settings.GetWindStrength(strength), 2);

        return (float)result;
    }

    private async void HandleReset()
    {
        try
        {
            var scene = GetTree().CurrentScene.SceneFilePath;

            // If you want to show the loading screen and fade:
            await SceneManager.ChangeScene(scene);
        }
        catch (System.Exception ex)
        {
            PrintErr($"Error: {ex.Message}\nCause: {ex.Source}");
            System.Environment.Exit(1);
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

        Print("IsStop " + ball.IsStop);

        if (ball.IsStop)
        {
            if (!_windApplied)
            {
                _windApplied = true; // Lock it so it doesn't run again until reset

                //TODO:DEBUG
                var randomWindStrength = (Settings.WindStrength)RandRange(0, 4);
                var randomDirection = (Settings.WindDirection)RandRange(0, 1);

                var getRandomWindStrength = GetRandomWindStrength(randomWindStrength);

                Settings.CurrentWindStrength = getRandomWindStrength;
                Settings.CurrentWindDirection = Settings.GetWindDirection(randomDirection);

                windLabel.Text = getRandomWindStrength.ToString();
                if (getRandomWindStrength == 0)
                    windDirectionLabel.Text = Settings.Cross;
                else
                    windDirectionLabel.Text = randomDirection.Equals(Settings.WindDirection.Left)
                        ? Settings.LeftArrow
                        : Settings.RightArrow;

                Print($"{randomWindStrength} {randomDirection}");
            }
        }
        else
        {
            // Reset the flag when the ball is moving, so it can trigger again next time it stops
            _windApplied = false;
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        GetTree().Paused = false;

        windSFX = GetNode<AudioStreamPlayer2D>("WindSFX");

        windLabel = GetNode<Label>("CanvasLayer/Main/WindPanel/Panel/HBoxContainer/Count");
        windLabel.Text = GetRandomWindStrength(Settings.WindStrength.Weak).ToString();

        windDirectionLabel = GetNode<RichTextLabel>(
            "CanvasLayer/Main/WindPanel/Panel/HBoxContainer/WindDirection"
        );
        windDirectionLabel.Text = Settings.LeftArrow;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        try
        {
            resetBtn.Pressed -= HandleReset;
            pauseBtn.Pressed -= HandlePause;

            QueueFree();
        }
        catch (System.Exception ex)
        {
            PrintErr($"Error: {ex.Message}\nCause: {ex.Source}");
            System.Environment.Exit(1);
        }
    }
}

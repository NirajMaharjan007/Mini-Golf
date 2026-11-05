using System;
using Godot;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Ball : RigidBody2D
{
    [Export]
    private float MaxPull = 150f; // max drag distance (px)

    [Export]
    private float PowerMultiplier = 8f; // launch speed = pull distance * this

    [Export]
    private float StopSpeed = 5f; // below this speed the ball re-freezes

    private bool _isDragging;
    private bool _launched;
    private Vector2 _anchor;

    private Line2D aimLine;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        aimLine = GetNode<Line2D>("Aim2D");
        if (aimLine != null)
        {
            aimLine.TopLevel = true;
            aimLine.GlobalPosition = Vector2.Zero;
            aimLine.GlobalScale = Vector2.One;
            aimLine.GlobalRotation = 0;
            aimLine.ClearPoints();
        }

        InputPickable = true; // default is true, but be explicit
        InputEvent += OnBodyInputEvent;
        FreezeMode = FreezeModeEnum.Kinematic;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_isDragging)
        {
            GlobalPosition = _anchor; // never moves while aiming
            UpdateAimLine();
        }
        else if (_launched)
        {
            CheckStopped();
        }
    }

    private void CheckStopped()
    {
        if (LinearVelocity.Length() < StopSpeed && GetContactCount() > 0)
        {
            _launched = false;
            Freeze = true;
            LinearVelocity = Vector2.Zero;
            AngularVelocity = 0;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (
            _isDragging
            && @event is InputEventMouseButton mb
            && mb.ButtonIndex == MouseButton.Left
            && !mb.Pressed
        )
        {
            Release();
        }
    }

    private void Release()
    {
        _isDragging = false;
        Vector2 pull = (GetGlobalMousePosition() - _anchor).LimitLength(MaxPull);
        Vector2 launchVel = -pull * PowerMultiplier;

        // Pulled too little: cancel the shot and put the ball back
        if (launchVel.Length() < 20f)
        {
            GlobalPosition = _anchor;
            return;
        }

        _launched = true;
        Freeze = false;

        aimLine.ClearPoints();
        SetDeferred(RigidBody2D.PropertyName.LinearVelocity, launchVel);
    }

    private void OnBodyInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (_isDragging || _launched)
            return;

        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
        {
            _isDragging = true;
            _anchor = GlobalPosition;

            Freeze = true;
            LinearVelocity = Vector2.Zero;
            AngularVelocity = 0;
        }
    }

    private void UpdateAimLine()
    {
        if (aimLine == null)
            return;

        const float BaseAimDistance = 300f; // line length at full pull
        const float MinAimDistance = 30f; // line length at near-zero pull
        const int Steps = 16;

        Vector2 pull = (GetGlobalMousePosition() - _anchor).LimitLength(MaxPull);

        float pullLen = pull.Length();

        if (pullLen < 2f)
        {
            aimLine.ClearPoints();
            return;
        }

        float gravity =
            (float)ProjectSettings.GetSetting("physics/2d/default_gravity") * GravityScale;

        Vector2 halfG = new(0f, 0.5f * gravity);

        Vector2 vel = -pull * PowerMultiplier;
        float speed = vel.Length(); // > 0 guaranteed by the check above

        float pullRatio = pullLen / MaxPull;
        float distance = Mathf.Lerp(MinAimDistance, BaseAimDistance, pullRatio);

        // Time to travel `distance` along the velocity direction (not just X)
        float tMax = distance / speed;
        float dt = tMax / Steps;

        var points = new Vector2[Steps + 1];
        for (int i = 0; i <= Steps; i++)
        {
            float t = i * dt;
            points[i] = _anchor + vel * t + halfG * t * t;
        }

        aimLine.Points = points;
    }

    /* 	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
        } */
}

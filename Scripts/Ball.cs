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

    [Export]
    private float StopTime = 0.25f;

    [Export]
    private float FlightLinearDamp = 0.6f; // air drag, higher = slows faster

    [Export]
    private float FlightAngularDamp = 1.0f; // spin slows down too

    private AudioStreamPlayer2D hit,
        bounce;

    private float _slowTimer;
    private bool _isDragging,
        _launched;
    private Vector2 _anchor;

    private Line2D aimLine;

    private Tween _hitTween;

    public int Counter { private set; get; } = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        hit = GetNode<AudioStreamPlayer2D>("Hit");

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
            CheckStopped((float)delta);
        }
    }

    private void CheckStopped(float delta)
    {
        // No contact requirement: just "slow for long enough"
        if (LinearVelocity.Length() < StopSpeed)
            _slowTimer += delta;
        else
            _slowTimer = 0f;

        if (_slowTimer >= StopTime)
        {
            _launched = false;
            _slowTimer = 0f;
            Freeze = true;
            LinearVelocity = Vector2.Zero;
            AngularVelocity = 0;
            LinearDamp = 0;
            AngularDamp = 0;
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
            Counter++;
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

        LinearDampMode = DampMode.Replace;
        AngularDampMode = DampMode.Replace;
        LinearDamp = FlightLinearDamp;
        AngularDamp = FlightAngularDamp;

        _launched = true;
        Freeze = false;

        aimLine.ClearPoints();

        CallDeferred(nameof(PlayHitSound));

        SetDeferred(RigidBody2D.PropertyName.LinearVelocity, launchVel);
    }

    private void PlayHitSound()
    {
        const float HitMinDb = -18f; // volume at weakest valid shot
        const float HitMaxDb = 0f; // volume at full pull
        const float HitFadeIn = 0.04f;
        const float HitFadeOut = 0.35f;

        _hitTween?.Kill();

        _hitTween = CreateTween();

        float pullRatio = GetPullRatio();

        float targetDb = Mathf.Lerp(HitMinDb, HitMaxDb, pullRatio);

        hit.PitchScale = (float)RandRange(0.95, 1.05) + pullRatio * 0.1f; // stronger = slightly higher
        hit.VolumeDb = -40f;
        hit.Play();

        // Quick fade in (removes the click)
        _hitTween
            .TweenProperty(hit, "volume_db", targetDb, HitFadeIn)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        // Smooth fade out toward the end
        _hitTween
            .TweenProperty(hit, "volume_db", -40f, HitFadeOut)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        _hitTween.TweenCallback(Callable.From(hit.Stop));
    }

    private void OnBodyInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (_isDragging)
            return;

        // Allow grabbing when at rest, OR when slow enough to count as stopped
        if (_launched && LinearVelocity.Length() > StopSpeed)
            return;

        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
        {
            // _slowTimer = 0f;
            _isDragging = true;
            _launched = false;
            _anchor = GlobalPosition;

            Freeze = true;
            LinearVelocity = Vector2.Zero;
            AngularVelocity = 0;
        }
    }

    private float GetPullRatio()
    {
        Vector2 pull = (GetGlobalMousePosition() - _anchor).LimitLength(MaxPull);

        return pull.Length() / MaxPull;
    }

    private bool IsTargetPull()
    {
        Vector2 pull = (GetGlobalMousePosition() - _anchor).LimitLength(MaxPull);

        return pull.Length() < 2f;
    }

    private void UpdateAimLine()
    {
        if (aimLine == null)
            return;

        const float BaseAimDistance = 300f; // line length at full pull
        const float MinAimDistance = 30f; // line length at near-zero pull
        const int Steps = 16;

        Vector2 pull = (GetGlobalMousePosition() - _anchor).LimitLength(MaxPull);

        if (IsTargetPull())
        {
            aimLine.ClearPoints();
            return;
        }

        float gravity =
            (float)ProjectSettings.GetSetting("physics/2d/default_gravity") * GravityScale;

        Vector2 halfG = new(0f, 0.5f * gravity);

        Vector2 vel = -pull * PowerMultiplier;
        float speed = vel.Length(); // > 0 guaranteed by the check above

        float pullRatio = GetPullRatio();
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

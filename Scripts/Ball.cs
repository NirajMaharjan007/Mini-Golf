using Godot;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Ball : RigidBody2D
{
	private const float TargetSpeedX = 4;
	private const float TargetAngular = 8;
	private const float Smoothing = 5f; // higher = faster response

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		float t = 1f - Mathf.Exp(-Smoothing * (float)state.Step); // frame-rate independent

		Vector2 targetLinear = new(TargetSpeedX, state.LinearVelocity.Y);
		state.LinearVelocity = state.LinearVelocity.Lerp(targetLinear, t);
		state.AngularVelocity = Mathf.Lerp(state.AngularVelocity, TargetAngular, t);
	}

}

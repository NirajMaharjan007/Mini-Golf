using Godot;
using static Godot.GD;

namespace MiniGolf.Scripts;

public partial class Ball : RigidBody2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    /* 	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
        } */
}

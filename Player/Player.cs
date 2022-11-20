using Godot;
using System;

public class Player : KinematicBody2D
{
	private const Int16 ACCELERATION = 700;
	private const Int16 FRICTION = 700;
	private const Int16 MAX_SPEED = 100;

	public Vector2 Velocity = Vector2.Zero;

	public override void _PhysicsProcess(float delta)
	{
		var inputVector = Vector2.Zero;

		// Calculating new position
		inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

		// Normalizing the positing in the viewport's cartesian plan
		inputVector = inputVector.Normalized();

		if (inputVector != Vector2.Zero)
	    	// Adding acceleration and capping to max speed
            Velocity = Velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * delta);
		else
	    	// Adding friction to not stop abruptely
		    Velocity = Velocity.MoveToward(Vector2.Zero, FRICTION * delta);

		GD.Print(Velocity);

		MoveAndCollide(Velocity * delta);
	}
}

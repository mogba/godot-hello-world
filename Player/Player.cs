using Godot;
using System;

public class Player : KinematicBody2D
{
	private const Int16 ACCELERATION = 700;
	private const Int16 FRICTION = 700;
	private const Int16 MAX_SPEED = 100;

	public Vector2 _velocity = Vector2.Zero;
	public AnimationPlayer _animationPlayer = null;
	public AnimationTree _animationTree = null;

	public override void _Ready()
	{
		// Gets node from the same scene
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationTree = GetNode<AnimationTree>("AnimationTree");
	}

	public override void _PhysicsProcess(float delta)
	{
		var inputDirection = GetInputDirectionStrength();

		MovePlayerPosition(inputDirection, delta);
		AnimatePlayer(inputDirection);
	}

	private void MovePlayerPosition(Vector2 inputDirection, float delta)
	{
		if (inputDirection != Vector2.Zero)
		{
			// Adding acceleration and capping to max speed
			_velocity = _velocity.MoveToward(inputDirection * MAX_SPEED, ACCELERATION * delta);
		}
		else
			// Adding friction to not stop abruptely
			_velocity = _velocity.MoveToward(Vector2.Zero, FRICTION * delta);

		_velocity = MoveAndSlide(_velocity);
	}

	private void AnimatePlayer(Vector2 inputDirection)
	{
		// Must update animation only when movement key is being pressed,
		// otherwise the start animation would be trigger.
		// For example, the player is running to the right and then stops. If
		// the animation was updated after that, when the key is not pressed 
		// anymore, then the new animation could be idle left, instead of facing
		// the right.
		if (inputDirection != Vector2.Zero)
		{
			_animationTree.Set("parameters/Idle/blend_position", inputDirection);
			_animationTree.Set("parameters/Run/blend_position", inputDirection);
		}
	}

	private Vector2 GetInputDirectionStrength()
	{
		var inputVector = Vector2.Zero;

		// Calculating new position
		inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

		// Normalizing the positing in the viewport's cartesian plan
		inputVector = inputVector.Normalized();

		return inputVector;
	}
}

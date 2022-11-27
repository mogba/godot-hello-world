using Godot;
using System;

public class Player : KinematicBody2D
{
	private const Int16 ACCELERATION = 700;
	private const Int16 FRICTION = 700;
	private const Int16 MAX_SPEED = 100;
	private const Int16 ROLL_SPEED = 120;

	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _rollDirection = Vector2.Zero;
	private AnimationPlayer _animationPlayer = null;
	private AnimationTree _animationTree = null;
	private AnimationNodeStateMachinePlayback _animationStateMachine = null;

	private AnimationStateType _animationState = AnimationStateType.MOVE;

	enum AnimationStateType
	{
		MOVE,
		ATTACK,
		ROLL,
	}

	public override void _Ready()
	{
		// Gets node from the same scene
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

		// Activate player animations
		_animationTree.Active = true;
	}

	public override void _PhysicsProcess(float delta)
	{
		var inputDirection = GetInputDirectionStrength();

		switch (_animationState)
		{
			case AnimationStateType.MOVE:
				AnimatePlayer(inputDirection, delta);

				if (Input.IsActionJustPressed("attack"))
				{
					_animationState = AnimationStateType.ATTACK;
				}
				if (Input.IsActionJustPressed("roll"))
				{
					_animationState = AnimationStateType.ROLL;
				}
				break;
			case AnimationStateType.ROLL:
				ExecuteStateRoll(inputDirection);
				break;
			case AnimationStateType.ATTACK:
				ExecuteStateAttack(inputDirection);
				break;
		}
	}

	public void OnRollAnimationFinished()
	{
		_velocity *= 0.6f;
		ResetAnimationStateToMove();
	}

	public void OnAttackAnimationFinished() => ResetAnimationStateToMove();

	private void ResetAnimationStateToMove() => _animationState = AnimationStateType.MOVE;

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

	private void AnimatePlayer(Vector2 inputDirection, float delta)
	{
		UpdatePlayerVelocity(inputDirection, delta);

		if (inputDirection != Vector2.Zero)
		{
			UpdateAnimationDirection(inputDirection);
			ExecuteStateRun(inputDirection);
		}
		else
		{
			ExecuteStateIdle(inputDirection);
		}

		MovePlayer();
	}

	private void UpdatePlayerVelocity(Vector2 inputDirection, float delta)
	{
		if (inputDirection != Vector2.Zero)
		{
			// Adding acceleration capped to max speed
			_velocity = _velocity.MoveToward(inputDirection * MAX_SPEED, ACCELERATION * delta);

			_rollDirection = inputDirection;
		}
		else
		{
			// Adding friction to not stop abruptely
			_velocity = _velocity.MoveToward(Vector2.Zero, FRICTION * delta);
		}
	}

	private void UpdateAnimationDirection(Vector2 inputDirection)
	{
		// Must update animation only when movement key is being pressed,
		// otherwise the start animation would be triggered.
		// For example, the player is running to the right and then stops: if
		// the animation was updated after the key stopped being pressed, then 
		// the new animation could be wrongly reset to idle down, instead 
		// of idle right.
		_animationTree.Set("parameters/Idle/blend_position", inputDirection);
		_animationTree.Set("parameters/Run/blend_position", inputDirection);
		_animationTree.Set("parameters/Roll/blend_position", inputDirection);
		_animationTree.Set("parameters/Attack/blend_position", inputDirection);
	}

	private void ExecuteStateIdle(Vector2 inputDirection)
		=> _animationStateMachine.Travel("Idle");

	private void ExecuteStateRun(Vector2 inputDirection)
		=> _animationStateMachine.Travel("Run");

	private void ExecuteStateRoll(Vector2 inputDirection)
	{
		_velocity = _rollDirection * ROLL_SPEED;
		_animationStateMachine.Travel("Roll");
		MovePlayer();
	}

	private void ExecuteStateAttack(Vector2 inputDirection)
	{
		// Clear velocity so the player doesn't 
		// continue to move after the animation.
		_velocity = Vector2.Zero;
		_animationStateMachine.Travel("Attack");
	}
	
	private void MovePlayer()
		=> _velocity = MoveAndSlide(_velocity);
}

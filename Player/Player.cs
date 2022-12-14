using Godot;

public class Player : KinematicBody2D
{
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _rollDirection = Vector2.Down;
	private SwordHitbox _swordHitbox = null;
	private AnimationStateType _animationState = AnimationStateType.MOVE;
	private Status _status = null;

	private AnimationTree _animationTree = null;
	private AnimationNodeStateMachinePlayback _animationStateMachine = null;
	
	[Export] public int Acceleration = 700;
	[Export] public int Friction = 700;
	[Export] public int MaxSpeed = 100;
	[Export] public int RollSpeed = 120;

	private enum AnimationStateType
	{
		MOVE,
		ATTACK,
		ROLL,
	}

	public override void _Ready()
	{
		// Gets node from the same scene
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
		_swordHitbox = GetNode<SwordHitbox>("HitboxPivot/SwordHitbox");

		// PlayStatus is an AutoLoad singleton. It's a global 
		// variable/node that is under the root tree an can be
		// accessed in any place just like any other normal node.
		// PS: In GDScript, AutoLoad variables can be accessed
		// directly by its name. It's a syntactic sugar.
		_status = GetNode<Status>("/root/PlayerStatus");

		// Activate player animations
		_animationTree.Active = true;

		// When referencing Godot's built-in functions by strings,
		// the functions' name must be just like in GDSCript, in
		// snake_case instead of the C# standard PascalCase. 
		_status.Connect("NoHealth", this, "queue_free");
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

	public void _OnHurtboxAreaEntered(Area2D area)
	{
		_status.Health -= 1;
	}

	public void OnRollAnimationFinished()
	{
		// Reduces the velocity so the player doesn't
		// slide too much after the animation is finished.
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
			_velocity = _velocity.MoveToward(inputDirection * MaxSpeed, Acceleration * delta);

			// Save last pressed movement direction to use on the roll animation
			_rollDirection = inputDirection;
			_swordHitbox.Knockback = _rollDirection;
		}
		else
		{
			// Adding friction to not stop abruptely
			_velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);
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
		// The direction of the roll animation cannot be changed.
		// When the roll animation starts, its direction is tighted to
		// the last direction value from the running animation.
		_velocity = _rollDirection * RollSpeed;
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

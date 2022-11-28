using Godot;

public class Bat : KinematicBody2D
{
	private static PackedScene _enemyDeathEffectScene =
		ResourceLoader.Load<PackedScene>("res://Effects/EnemyDeathEffect/EnemyDeathEffect.tscn");

	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _knockback = Vector2.Zero;
	private AnimationStateType _animationState = AnimationStateType.CHASE;

	private AnimatedSprite _animatedSprite = null;
	private Status _status = null;
	private PlayerDetectionZone _playerDetectionZone = null;

	[Export] public int KnockbackForce = 200;
	[Export] public int Acceleration = 300;
	[Export] public int Friction = 200;
	[Export] public int MaxSpeed = 50;

	private enum AnimationStateType
	{
		IDLE,
		WANDER,
		CHASE
	}

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_status = GetNode<Status>("Status");
		_playerDetectionZone = GetNode<PlayerDetectionZone>("PlayerDetectionZone");

		_animatedSprite.Frame = 0;
		_animatedSprite.Play("Animate");
	}

	public override void _PhysicsProcess(float delta)
	{
		_knockback = _knockback.MoveToward(Vector2.Zero, KnockbackForce * delta);
		_knockback = MoveAndSlide(_knockback);

		switch (_animationState)
		{
			case AnimationStateType.IDLE:
				_velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);
				SeekAndChasePlayer();
				break;
			case AnimationStateType.WANDER:
				break;
			case AnimationStateType.CHASE:
				if (_playerDetectionZone.IsPlayerDetected())
				{
					var newPosition = (_playerDetectionZone.Player.GlobalPosition - GlobalPosition).Normalized();
					_velocity = _velocity.MoveToward(newPosition * MaxSpeed, Acceleration * delta);

					_animatedSprite.FlipH = _velocity.x < 0;
				}
				else
				{
					_animationState = AnimationStateType.IDLE;
				}
				break;
		}

		_velocity = MoveAndSlide(_velocity);
	}

	public void _OnHurtboxAreaEntered(Area2D area)
	{
		var swordHitbox = (area as SwordHitbox);
		_status.Health -= swordHitbox.Damage;
		_knockback = swordHitbox.Knockback * 120;
	}

	public void _OnStatusNoHealth()
	{
		CreateEnemyDeathEffect();
		QueueFree();
	}

	private void CreateEnemyDeathEffect()
		=> _enemyDeathEffectScene.Instance<Effect>().AttachToRoot(this);

	private void SeekAndChasePlayer()
	{
		if (_playerDetectionZone.IsPlayerDetected())
		{
			_animationState = AnimationStateType.CHASE;
		}
	}
}

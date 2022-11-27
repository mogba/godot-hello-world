using Godot;

public class Bat : KinematicBody2D
{
	private static PackedScene _enemyDeathEffectScene =
		ResourceLoader.Load<PackedScene>("res://Effects/EnemyDeathEffect/EnemyDeathEffect.tscn");

	private Vector2 _knockback = Vector2.Zero;
	private int _knockbackForce = 200;
	private AnimatedSprite _animatedSprite = null;
	private Status _status = null;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_status = GetNode<Status>("Status");

		_animatedSprite.Frame = 0;
		_animatedSprite.Play("Animate");
	}

	public override void _PhysicsProcess(float delta)
	{
		_knockback = _knockback.MoveToward(Vector2.Zero, _knockbackForce * delta);
		_knockback = MoveAndSlide(_knockback);
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

	private void CreateEnemyDeathEffect() => _enemyDeathEffectScene.Instance<Effect>().AttachToRoot(this);
}

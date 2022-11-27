using Godot;

public class Bat : KinematicBody2D
{
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
	}

	// TODO: Refactor
	private void CreateEnemyDeathEffect()
	{
		// Starts the dying animation before erasing the enemy
		var deathEffectScene = GD.Load<PackedScene>("res://Effects/EnemyDeathEffect/EnemyDeathEffect.tscn");
		var deathEffectInstance = deathEffectScene.Instance<EnemyDeathEffect>();

		// Sets the effect's position to the same as the enemy's position
		deathEffectInstance.GlobalPosition = GlobalPosition;
		// deathEffectInstance.SetKnockback(_knockback, _knockbackForce);

		// Add the effect's instance to the current scene
		var world = GetTree().CurrentScene;
		world.AddChild(deathEffectInstance);

		deathEffectInstance.Connect("AnimationFinished", this, "RemoveEnemy");

		// This enemy has to be erase from the 
		// world when the death animation ends
	}

	public void RemoveEnemy()
	{
		QueueFree();
	}
}

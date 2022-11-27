using Godot;

public class EnemyDeathEffect : Node2D
{
    [Signal] delegate void AnimationFinished();

    private Vector2 _knockback = Vector2.Zero;
    private int _knockbackForce = 0;

    public override void _Ready()
    {
        var animatedSprite = GetNode<AnimationPlayer>("AnimationPlayer");

		animatedSprite.Play("Animate");

		animatedSprite = null;
    }

	// public override void _PhysicsProcess(float delta)
	// {
	// 	_knockback = _knockback.MoveToward(Vector2.Zero, _knockbackForce * delta);
	// 	Position = _knockback;
	// }

    public void OnAnimationPlayerAnimationFinished()
    {
        EmitSignal("AnimationFinished");
        QueueFree();
    }

    public void SetKnockback(Vector2 knockback, int force)
    {
        _knockback = knockback;
        _knockbackForce = force;
    }
}

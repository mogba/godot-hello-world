using Godot;

public class GrassEffect : Node2D
{
	public override void _Ready()
	{
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		// Reset and play animation
		animatedSprite.Frame = 0;
		animatedSprite.Play("Animate");

		animatedSprite = null;
	}

    // Method attached to the animation_finished signal from AnimatedSprite.
    // Signals are like events which triggering can be listened to, thus
    // executing an action (method) when the event is triggered.
	public void _OnAnimatedSpriteAnimationFinished()
	{
        QueueFree();
	}
}

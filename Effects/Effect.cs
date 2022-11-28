using Godot;

public class Effect : AnimatedSprite
{
    public override void _Ready()
	{
		// Reset and play the animation
		Frame = 0;
		Play(Animation);

        // Connects signal to nodes by code:
        // 1. Node with the signal;
        // 2. The signal;
        // 3. Target node that will receive the connection;
        // 4. Method from the target node that will be executed.
        this.Connect("animation_finished", this, "_OnAnimatedSpriteAnimationFinished");
	}

    // Method attached to the animation_finished signal from AnimatedSprite.
    // Signals are like events which triggering can be listened to, thus
    // executing an action (method) when the event is triggered.
	public void _OnAnimatedSpriteAnimationFinished()
	{
        QueueFree();
	}

    public void AttachToRoot(Node2D node)
    {
        GlobalPosition = node.GlobalPosition;
        // node.GetTree().CurrentScene -> gets the root node
		node.GetParent().AddChild(this);
    }
}

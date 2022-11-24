using Godot;

public class Grass : Node2D
{
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("attack"))
		{
			var scene = GD.Load<PackedScene>("res://Effects/GrassEffect.tscn");
			var instance = scene.Instance<GrassEffect>();
			instance.GlobalPosition = GlobalPosition;

			var world = GetTree().CurrentScene;
			world.AddChild(instance);

			// Deletes this grass' instance after the current frame's execution
			QueueFree();
		}
	}
}

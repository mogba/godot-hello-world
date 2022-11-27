using Godot;

public class Grass : Node2D
{
	public void _OnHurtboxAreaEntered(Area area)
	{
		CreateGrassEffect();
		QueueFree();
	}

	private void CreateGrassEffect()
	{
		var scene = GD.Load<PackedScene>("res://Effects/GrassEffect/GrassEffect.tscn");
		var instance = scene.Instance<GrassEffect>();
		instance.GlobalPosition = GlobalPosition;

		var world = GetTree().CurrentScene;
		world.AddChild(instance);

		// Deletes this grass' instance after the current frame's execution
		QueueFree();
	}
}

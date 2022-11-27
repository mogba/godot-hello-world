using Godot;

public class Grass : Node2D
{
	private static PackedScene _grassEffectScene =
		ResourceLoader.Load<PackedScene>("res://Effects/GrassEffect/GrassEffect.tscn");

	public void _OnHurtboxAreaEntered(Area area)
	{
		CreateGrassEffect();
		QueueFree();
	}

	private void CreateGrassEffect() => _grassEffectScene.Instance<Effect>().AttachToRoot(this);
}

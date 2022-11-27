using Godot;

public class Hurtbox : Area2D
{
	private static PackedScene _hitEffect =
        ResourceLoader.Load<PackedScene>("res://Effects/HitEffect/HitEffect.tscn");

    [Export] public bool ShowHitEffect = true;
    
    public void _OnHurtboxAreaEntered(Area2D area)
    {
        if (ShowHitEffect)
        {
            var hitEffectInstance = _hitEffect.Instance<Effect>();
            hitEffectInstance.GlobalPosition = GlobalPosition;
            GetTree().CurrentScene.AddChild(hitEffectInstance);
        }
    }
}

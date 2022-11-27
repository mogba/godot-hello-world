using Godot;

public class Status : Node
{
	[Signal] delegate void NoHealth();

    private int _health = 0;

	[Export] public int MaxHealth = 1;
	[Export] public int Health
	{
		get => _health;
		set => SetHealth(value);
	}

	public override void _Ready()
	{
		_health = MaxHealth;
	}

	private void SetHealth(int value)
	{
		_health = value;

		if (Health == 0)
		{
			EmitSignal("NoHealth");
		}
	}
}

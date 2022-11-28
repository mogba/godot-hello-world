using Godot;

public class PlayerDetectionZone : Area2D
{
    public Player Player { get; private set; }

    public void _OnPlayerDetectionZoneBodyEntered(Player body) => Player = body;

    public void _OnPlayerDetectionZoneBodyExited(Player _) => Player = null;

    public bool IsPlayerDetected() => Player != null;
}

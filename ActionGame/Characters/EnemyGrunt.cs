using Godot;
using System;

public partial class EnemyGrunt : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;

    public const float SPEED = 100.0f;

    public override void _Ready()
    {
        GD.Print("ENEMY GRUNT SPAWNED");
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Level.Player != null)
        {
            GD.Print("ENEMY GRUNT SPAWNED");
            Vector2 direction = Level.Player.Position - Position;
            Velocity = direction.Normalized() * SPEED;
            MoveAndSlide();
        }

    }
}

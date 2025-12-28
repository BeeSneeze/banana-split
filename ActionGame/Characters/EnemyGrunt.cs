using Godot;
using System;

public partial class EnemyGrunt : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;
    public int Knockbackframes = 0;

    private const float ACCELERATION = 8.0f;
    private const float MAX_SPEED = 150f;

    public override void _Ready()
    {
        // TODO: Fix references to room and level
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Level.Player != null)
        {
            Vector2 direction = Level.Player.Position - Position;

            if (Knockbackframes > 0)
            {
                Knockbackframes--;
            }
            else
            {
                Velocity += direction.Normalized() * ACCELERATION;
            }
            if (Velocity.Length() > MAX_SPEED && Knockbackframes < 1)
            {
                Velocity = Velocity.Normalized() * MAX_SPEED;
            }

            MoveAndSlide();
        }

    }
}

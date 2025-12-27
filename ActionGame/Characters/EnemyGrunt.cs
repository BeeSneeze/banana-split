using Godot;
using System;

public partial class EnemyGrunt : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;

    public int KnockBackFrames = 0;

    public const float SPEED = 100.0f;

    public override void _Ready()
    {
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Level.Player != null)
        {
            Vector2 direction = Level.Player.Position - Position;
            if (KnockBackFrames > 0)
            {
                KnockBackFrames--;
            }
            else
            {
                Velocity = direction.Normalized() * SPEED;
            }

            MoveAndSlide();
        }

    }
}

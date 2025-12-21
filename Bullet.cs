using Godot;
using System;
using Common;

public partial class Bullet : AnimatableBody2D
{
    private Team team;

    public Vector2 Velocity = new Vector2(0, 0);

    public override void _Ready()
    {
        if (CollisionLayer == 0)
        {
            throw new Exception("Bullet was created without a team!");
        }

        GetNode<AnimatedSprite2D>("Visual").Rotation = Mathf.Atan2(Velocity.Y, Velocity.X);
    }

    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(Velocity);
        if (collision != null)
        {
            QueueFree();
        }
    }

    public void SetTeam(Team setAllegiance)
    {

        team = setAllegiance;
        if (team == Team.ENEMY)
        {
            CollisionLayer = 16;
            CollisionMask = 2 + 32; // Player and walls
        }
        else if (team == Team.PLAYER)
        {
            CollisionLayer = 4;
            CollisionMask = 8 + 32;// Enemy and walls
        }
    }
}

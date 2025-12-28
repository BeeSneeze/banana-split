using Godot;
using System;
using Common;

public partial class Bullet : AnimatableBody2D
{
    public Room Room;
    public Vector2 Velocity = new Vector2(0, 0);

    private Team Team;

    public override void _Ready()
    {
        if (CollisionLayer == 0)
        {
            throw new Exception("Bullet was created without a Team!");
        }

        GetNode<AnimatedSprite2D>("Visual").Rotation = Mathf.Atan2(Velocity.Y, Velocity.X);
    }

    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(Velocity);
        if (collision != null)
        {
            var collider = (Node)collision.GetCollider();
            if (collider.IsInGroup("CanBeKnockedBack"))
            {
                var body = (EnemyGrunt)collider;
                body.Knockbackframes = 10;
                body.Velocity += Velocity * 25;
            }
            Room.SpawnParticle(ParticleNames.Explosion, Position);
            QueueFree();
        }
    }

    public void SetTeam(Team setAllegiance)
    {
        Team = setAllegiance;
        if (Team == Team.ENEMY)
        {
            CollisionLayer = ((uint)CollisionLayerDefs.ENEMY_BULLETS);
            CollisionMask = ((uint)CollisionLayerDefs.PLAYER) + ((uint)CollisionLayerDefs.WALLS);
        }
        else if (Team == Team.PLAYER)
        {
            CollisionLayer = ((uint)CollisionLayerDefs.PLAYER_BULLETS);
            CollisionMask = ((uint)CollisionLayerDefs.ENEMY) + ((uint)CollisionLayerDefs.WALLS);
        }
    }
}

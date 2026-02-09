using Godot;
using System;
using Common;

public partial class Bullet : StaticBody2D
{
    public Room Room { get; private set; }
    public Vector2 Velocity { get; private set; }
    public DamageType damageType { get; private set; }
    public int DamageAmount { get; private set; }
    private Team Team;

    public override void _Ready()
    {
        if (CollisionLayer == 0)
        {
            throw new Exception("Bullet was created without a Team!");
        }

        GetNode<AnimatedSprite2D>("Visual").Rotation = Mathf.Atan2(Velocity.Y, Velocity.X);
    }

    public void Initialize(Room room, Vector2 velocity, DamageType damagetype, int damage, Team team)
    {
        SetTeam(team);
        Room = room;
        Velocity = velocity;
        damageType = damagetype;
        DamageAmount = damage;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Generally speaking, entities listen to bullets to resolve collisions
        // The exception are walls which the bullet keeps track of itself
        var collision = MoveAndCollide(Velocity);
        if (collision != null)
        {
            ExplodeBullet();
        }
    }

    // To be used by entities when they get hit by bullets
    public void ExplodeBullet()
    {
        if (Team == Team.PLAYER)
        {
            Room.SpawnParticle(ParticleNames.Splash, Position);
        }
        else
        {
            Room.SpawnParticle(ParticleNames.Explosion, Position);
        }
        QueueFree();
    }

    private void SetTeam(Team setAllegiance)
    {

        var visual = GetNode<AnimatedSprite2D>("Visual");
        Team = setAllegiance;
        if (Team == Team.ENEMY)
        {
            visual.Animation = "Enemy";
            CollisionLayer = (uint)CollisionLayerDefs.ENEMY_BULLETS;
        }
        else if (Team == Team.PLAYER)
        {
            GetNode<PointLight2D>("Light").Visible = false;
            visual.Animation = "Player";
            CollisionLayer = (uint)CollisionLayerDefs.PLAYER_BULLETS;
        }
        visual.Play();
    }
}

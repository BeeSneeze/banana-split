using System;
using Godot;

public partial class EnemyBlaster : Enemy
{
    protected override float MAX_SPEED => 150f;
    protected override float BULLET_SPEED => 3f;
    protected override int MAX_HEALTH => 4;
    protected override int MAX_KNOCKBACK_FRAMES => 15;

    private int BulletCountdown;

    public override void _Ready()
    {
        BulletCountdown = 20;
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(1, 0);
        direction = direction.Rotated(GD.Randf() * (float)Math.Tau);

        if (BulletCountdown > 0)
        {
            BulletCountdown--;
        }
        else
        {
            SpawnBullet(direction);
            SpawnBullet(direction.Rotated(Mathf.Pi));
            BulletCountdown = 20;
        }

        if (Knockbackframes > 0)
        {
            ResolveKnockback();
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        ResolveMovement();
    }
}

using System;
using Common;
using Godot;

public partial class EnemyBlaster : Enemy
{
    protected override float MAX_SPEED => 150f;
    protected override float BULLET_SPEED => 4f;
    protected override int MAX_HEALTH => 7;
    protected override int MAX_KNOCKBACK_FRAMES => 15;
    protected override DamageType DAMAGETYPE => DamageType.Text;
    protected override float KNOCKBACK_MULTIPLIER { get; } = 5;

    private Vector2 ShootDirection = new Vector2(1, 0);

    protected override void ResetBulletCountdown()
    {
        BulletCountdown = 20;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (BulletCountdown > 0)
        {
            BulletCountdown--;
        }
        else
        {
            ShootDirection = ShootDirection.Rotated(0.2f);
            SpawnBullet(ShootDirection);
            SpawnBullet(ShootDirection.Rotated(Mathf.Pi));
            ResetBulletCountdown();
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

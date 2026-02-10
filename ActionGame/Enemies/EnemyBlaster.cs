using System;
using Common;
using Godot;

public partial class EnemyBlaster : Enemy
{
    protected override float MAX_SPEED => 150f;
    protected override float BULLET_SPEED => 3f;
    protected override int MAX_HEALTH => 7;
    protected override int MAX_KNOCKBACK_FRAMES => 15;
    protected override DamageType DAMAGETYPE => DamageType.Text;

    protected override void ResetBulletCountdown()
    {
        BulletCountdown = 20;
    }

    public override void _PhysicsProcess(double delta)
    {

        Vector2 direction = new Vector2(1, 0);
        if (Level.Player != null)
        {
            direction = Level.Player.GetPositionRelativeToRoom() - Position;
            GetNode<AnimatedSprite2D>("Visual").FlipH = direction.X < 0;
        }
        direction = direction.Rotated((float)GD.RandRange(-0.6, 0.6));

        if (BulletCountdown > 0)
        {
            BulletCountdown--;
        }
        else
        {
            SpawnBullet(direction);
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

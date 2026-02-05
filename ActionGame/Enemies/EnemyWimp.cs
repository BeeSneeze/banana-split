using Godot;
using Common;

public partial class EnemyWimp : Enemy
{
    protected override float MAX_SPEED => 100f;
    protected override float BULLET_SPEED => 7f;
    protected override int MAX_HEALTH => 4;
    protected override int MAX_KNOCKBACK_FRAMES => 20;
    protected override DamageType DAMAGETYPE => DamageType.Arrows;

    private const float WIMP_DISTANCE = 400f;

    protected override void ResetBulletCountdown()
    {
        BulletCountdown = 150 + GD.RandRange(-100, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(0, 0);
        if (Level.Player != null)
        {
            direction = Level.Player.GetPositionRelativeToRoom() - Position;
            GetNode<AnimatedSprite2D>("Visual").FlipH = direction.X < 0;
        }

        if (BulletCountdown > 0)
        {
            BulletCountdown--;
        }
        else
        {
            SpawnBullet(direction.Rotated((float)GD.RandRange(-0.2, 0.2)));
            ResetBulletCountdown();
        }

        if (Knockbackframes > 0)
        {
            ResolveKnockback();
        }
        else
        {
            if (direction.Length() > WIMP_DISTANCE * 2)
            {
                Velocity += direction.Normalized() * ACCELERATION * 0.1f;
            }
            else if (direction.Length() > WIMP_DISTANCE)
            {
                Velocity += direction.Normalized() * ACCELERATION;
            }
            else
            {
                Velocity -= direction.Normalized() * ACCELERATION * 0.7f;
            }
        }

        ResolveMovement();
    }



}

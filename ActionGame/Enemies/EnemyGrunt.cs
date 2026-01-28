using Godot;
using Common;

public partial class EnemyGrunt : Enemy
{
    protected override float MAX_SPEED => 190f;
    protected override float BULLET_SPEED => 10f;
    protected override int MAX_HEALTH => 5;
    protected override int MAX_KNOCKBACK_FRAMES => 15;
    protected override DamageType DAMAGETYPE => DamageType.Scramble;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(0, 0);
        if (Level.Player != null)
        {
            direction = Level.Player.GetPositionRelativeToRoom() - Position;
            GetNode<AnimatedSprite2D>("Visual").FlipH = direction.X < 0;
        }

        if (Knockbackframes > 0)
        {
            ResolveKnockback();
        }
        else
        {
            Velocity += direction.Normalized() * ACCELERATION;
        }

        ResolveMovement();
    }
}

using Godot;

public partial class EnemyGrunt : Enemy
{
    protected override float MAX_SPEED => 150f;
    protected override float BULLET_SPEED => 10f;
    protected override int MAX_HEALTH => 4;
    protected override int MAX_KNOCKBACK_FRAMES => 15;

    private const float ACCELERATION = 8.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(0, 0);
        if (Level.Player != null)
        {
            direction = Level.Player.Position - Position;
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

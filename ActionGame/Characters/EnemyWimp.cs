using Godot;

public partial class EnemyWimp : Enemy
{
    protected override float MAX_SPEED => 150f;
    protected override float BULLET_SPEED => 10f;
    protected override int MAX_HEALTH => 5;

    private int BulletCountdown;
    private const float ACCELERATION = 8.0f;
    private const float WIMP_DISTANCE = 400f;

    public override void _Ready()
    {
        BulletCountdown = 200 + GD.RandRange(-40, 40);
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(0, 0);
        if (Level.Player != null)
        {
            direction = Level.Player.Position - Position;
        }

        if (BulletCountdown > 0)
        {
            BulletCountdown--;
        }
        else
        {
            SpawnBullet(direction);
            BulletCountdown = 200 + GD.RandRange(-40, 40);
        }

        if (Knockbackframes > 0)
        {
            Knockbackframes--;
        }
        else
        {
            if (direction.Length() > WIMP_DISTANCE)
            {
                Velocity += direction.Normalized() * ACCELERATION;
            }
            else
            {
                Velocity -= direction.Normalized() * ACCELERATION;
            }
        }

        ResolveMovement();
    }

}

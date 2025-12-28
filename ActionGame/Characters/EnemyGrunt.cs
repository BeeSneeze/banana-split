using Godot;

public partial class EnemyGrunt : Enemy
{
    private const float ACCELERATION = 8.0f;
    private const float MAX_SPEED = 150f;

    public override void _Ready()
    {
        HealthPoints = 5;
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Level.Player != null)
        {
            Vector2 direction = Level.Player.Position - Position;

            if (Knockbackframes > 0)
            {
                Knockbackframes--;
            }
            else
            {
                Velocity += direction.Normalized() * ACCELERATION;
            }
            if (Velocity.Length() > MAX_SPEED && Knockbackframes < 1)
            {
                Velocity = Velocity.Normalized() * MAX_SPEED;
            }

            MoveAndSlide();
        }

    }
}

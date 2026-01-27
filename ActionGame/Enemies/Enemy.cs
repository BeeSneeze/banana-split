using Common;
using Godot;

public abstract partial class Enemy : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;

    protected int Knockbackframes = 0;
    protected int HealthPoints;
    protected int BulletCountdown;
    protected Vector2 KnockBackVelocity;

    protected abstract int MAX_KNOCKBACK_FRAMES { get; }
    protected abstract int MAX_HEALTH { get; }
    protected abstract float MAX_SPEED { get; }
    protected abstract float BULLET_SPEED { get; }

    protected virtual float ACCELERATION { get; } = 8.0f;

    private const int KNOCKBACK_DELAY = 3;
    private PackedScene BulletScene;

    public override void _Ready()
    {
        HealthPoints = MAX_HEALTH;
        BulletScene = GD.Load<PackedScene>("res://ActionGame/bullet.tscn");
        // TODO: Fix references to room and level
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
        SetPhysicsProcess(false);
    }

    private void TakeHit(Vector2 incomingVelocity)
    {
        KnockBackVelocity = incomingVelocity * 25;
        Knockbackframes = MAX_KNOCKBACK_FRAMES;
        HealthPoints--;
        if (HealthPoints < 1)
        {
            QueueFree();
        }
        Modulate = new Color(0.5f, 1.5f, 5.0f);
        Scale = new Vector2(0.8f, 0.8f);
        var newTween = GetTree().CreateTween();
        newTween.TweenProperty(this, "modulate", new Color(1, 1, 1), 0.12);
        var newTween2 = GetTree().CreateTween();
        newTween2.TweenProperty(this, "scale", new Vector2(1, 1), 0.12);
    }

    protected void ResolveKnockback()
    {
        Knockbackframes--;
        if (Knockbackframes > MAX_KNOCKBACK_FRAMES - KNOCKBACK_DELAY)
        {
            Velocity = Vector2.Zero;
        }
        else if (Knockbackframes == MAX_KNOCKBACK_FRAMES - KNOCKBACK_DELAY)
        {
            Velocity += KnockBackVelocity;
        }
    }

    protected void ResolveMovement()
    {
        if (Velocity.Length() > MAX_SPEED && Knockbackframes < 1)
        {
            Velocity = Velocity.Normalized() * MAX_SPEED;
        }
        MoveAndSlide();

        var collision = GetLastSlideCollision();
        if (collision != null)
        {
            var collider = collision.GetCollider();
            if (collider is Bullet bullet)
            {
                TakeHit(bullet.Velocity);
                bullet.ExplodeBullet();
            }

            if (collider is Player player)
            {
                player.TakeDamage(2, MinigameBoxType.Text);
            }
        }
    }

    public void CreatePersonalSpace(Enemy otherEnemy)
    {
        var directionVector = otherEnemy.Position - Position;
        Velocity -= directionVector.Normalized() * ACCELERATION * 1.3f;
    }

    protected void SpawnBullet(Vector2 direction)
    {
        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.ENEMY);

        var random = new RandomNumberGenerator();
        random.Randomize();

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-0.05f, 0.05f));
        newBullet.Room = Room;
        Room.Spawn(newBullet, Position);
    }
}

using Common;
using Godot;

public partial class Enemy : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;
    public int Knockbackframes = 0;
    protected int HealthPoints;
    protected float BULLET_SPEED;
    private PackedScene BulletScene;

    public override void _Ready()
    {
        BulletScene = GD.Load<PackedScene>("res://ActionGame/bullet.tscn");
        // TODO: Fix references to room and level
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
    }

    public void TakeHit(Vector2 incomingVelocity)
    {
        Velocity += incomingVelocity * 25;
        Knockbackframes = 10;
        HealthPoints--;
        if (HealthPoints < 1)
        {
            GD.Print("GRUNT DIED!");
            QueueFree();
        }
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

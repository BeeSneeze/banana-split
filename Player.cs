using Godot;
using System;
using Common;

public partial class Player : CharacterBody2D
{
    private ActionGame ActionGame;
    private PackedScene BulletScene;
    private bool Initiated = false;

    private int HealthPoints = 10;

    private const float PLAYER_SPEED = 300.0f;
    private const float BULLET_SPEED = 13.0f;
    private const int I_FRAME_COUNT = 60;
    private const int BULLET_SPAWN_TIME = 8;
    private const double RELOAD_TIME = 1.5;
    private const int MAX_BULLETS_IN_CHAMBER = 6;

    private double Reload = 0;
    private int BulletsInChamber = 6;
    private int invincibilityFrames = 0;
    private int shootFrames = 0;

    public override void _Ready()
    {
        if (!Initiated)
        {
            throw new Exception("PLAYER WAS NOT INITIATED!");
        }
        BulletScene = GD.Load<PackedScene>("res://bullet.tscn");
        AdjustHp(HealthPoints);
    }

    public void Initiate(ActionGame actionGame)
    {
        ActionGame = actionGame;
        Initiated = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        invincibilityFrames--;
        shootFrames--;
        Vector2 direction = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");

        if (BulletsInChamber == 0)
        {
            Reload -= delta;
            if (Reload <= 0)
            {
                BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
            }
        }

        // You can either shoot, or move, but not both!
        if (Input.IsActionPressed("GameShoot"))
        {
            if (shootFrames < 0 && direction != new Vector2(0, 0) && BulletsInChamber > 0)
            {
                shootFrames = BULLET_SPAWN_TIME;
                SpawnBullet(direction);
            }
        }
        else
        {
            Velocity = direction * PLAYER_SPEED;
            var collisionHappened = MoveAndSlide();

            if (collisionHappened)
            {
                var res = GetLastSlideCollision();

                var collision = (Node2D)res.GetCollider();
                if (collision.IsInGroup("DamagesPlayer"))
                {
                    if (invincibilityFrames < 0)
                    {
                        HealthPoints--;

                        AdjustHp(HealthPoints);
                    }
                }
            }
        }

    }

    private void SpawnBullet(Vector2 direction)
    {
        BulletsInChamber--;
        if (BulletsInChamber == 0)
        {
            Reload = RELOAD_TIME;
        }
        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.PLAYER);

        var random = new RandomNumberGenerator();
        random.Randomize();

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-0.05f, 0.05f));
        ActionGame.Spawn(newBullet, Position);
    }

    private void AdjustHp(int amount)
    {
        invincibilityFrames = I_FRAME_COUNT;
        CustomEvents.Instance.EmitSignal("AdjustHP", amount);
    }
}

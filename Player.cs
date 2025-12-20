using Godot;
using System;
using Common;

public partial class Player : CharacterBody2D
{
    [Export]
    private Game Game;
    private ActionGame ActionGame;
    private PackedScene BulletScene;

    private int invincibilityFrames = 0;
    private int shootFrames = 0;

    private int HealthPoints = 10;

    private const float PLAYER_SPEED = 300.0f;
    private const float BULLET_SPEED = 13.0f;

    private const int I_FRAME_COUNT = 60;
    private const int BULLET_SPAWN_TIME = 8;

    private const double RELOAD_TIME = 1.5;
    private double Reload = 0;
    private int BulletsInChamber = 6;
    private const int MAX_BULLETS_IN_CHAMBER = 6;


    public override void _Ready()
    {
        BulletScene = GD.Load<PackedScene>("res://bullet.tscn");
        // TODO: Fix this reference to the game
        Game = GetParent().GetParent<Game>();
        ActionGame = GetParent<ActionGame>();
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
        var aim = direction.Normalized();

        var random = new RandomNumberGenerator();
        random.Randomize();

        aim = aim.Rotated(random.RandfRange(-0.05f, 0.05f));

        newBullet.Velocity = aim * BULLET_SPEED;
        newBullet.Position = Position;
        ActionGame.SpawnBullet(newBullet);
    }

    private void AdjustHp(int amount)
    {
        invincibilityFrames = I_FRAME_COUNT;
        Game.AdjustHp(amount);
    }
}

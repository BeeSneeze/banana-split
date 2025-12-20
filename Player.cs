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

    private const float PLAYER_SPEED = 330.0f;
    private const float BULLET_SPEED = 650.0f;

    private const int I_FRAME_COUNT = 100;
    private const int BULLET_SPAWN_TIME = 10;

    private const float RELOAD_TIME = 1f;
    private float Reload = 0f;


    public override void _Ready()
    {
        BulletScene = GD.Load<PackedScene>("res://bullet.tscn");
        // TODO: Fix this reference to the game
        Game = GetParent().GetParent<Game>();
        ActionGame = GetParent<ActionGame>();
    }


    public override void _PhysicsProcess(double delta)
    {
        //Reload -= delta;
        invincibilityFrames--;
        shootFrames--;
        Vector2 direction = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");

        // You can either shoot, or move, but not both!
        if (Input.IsActionPressed("GameShoot"))
        {
            if (shootFrames < 0 && direction != new Vector2(0, 0))
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
        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.PLAYER);
        var aim = direction.Normalized();

        var random = new RandomNumberGenerator();
        random.Randomize();

        aim = aim.Rotated(random.RandfRange(-0.1f, 0.1f));

        newBullet.LinearVelocity = aim * BULLET_SPEED;
        newBullet.GlobalPosition = GlobalPosition;
        ActionGame.SpawnBullet(newBullet);
    }

    private void AdjustHp(int amount)
    {
        invincibilityFrames = I_FRAME_COUNT;
        Game.AdjustHp(amount);
    }
}

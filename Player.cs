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
    private const float BULLET_SPEED = 450.0f;

    private const int I_FRAME_COUNT = 100;
    private const int BULLET_SPAWN_TIME = 30;

    private Vector2 LastBulletDirection = new Vector2(1, 0);

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

        if (aim == new Vector2(0, 0))
        {
            aim = LastBulletDirection;
        }
        else
        {
            LastBulletDirection = aim;
        }

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

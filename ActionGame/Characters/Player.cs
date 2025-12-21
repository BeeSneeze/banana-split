using Godot;
using System;
using Common;

public partial class Player : CharacterBody2D
{
    private ActionGame Level;
    private PackedScene BulletScene;
    private AnimatedSprite2D Visual;
    private bool Initiated = false;

    private int HealthPoints = 10;

    private const float PLAYER_SPEED = 300.0f;
    private const float BULLET_SPEED = 13.0f;
    private const int I_FRAME_COUNT = 60;
    private const int BULLET_SPAWN_TIME = 8;
    private const double RELOAD_TIME = 1.5;
    private const int MAX_BULLETS_IN_CHAMBER = 6;
    private const int DODGE_I_FRAME_COUNT = 30;

    private double ReloadCountdown = 0;
    private int BulletsInChamber = 6;
    private int invincibilityFrames = 0;
    private int BulletSpawnCountdown = 0;
    private int DodgeCountdown = 0;
    private Vector2 DodgeDirection = new Vector2(0, 0);

    public override void _Ready()
    {
        if (!Initiated)
        {
            throw new Exception("PLAYER WAS NOT INITIATED!");
        }
        Visual = GetNode<AnimatedSprite2D>("Visual");
        BulletScene = GD.Load<PackedScene>("res://bullet.tscn");
        AdjustHp(HealthPoints);
    }

    public void Initiate(ActionGame level)
    {
        Level = level;
        Initiated = true;
    }

    private enum Visuals
    {
        RUNNING,
        GUNNING,
        DODGING,
        IDLING
    }

    private void SetVisual(Visuals vis)
    {
        switch (vis)
        {
            case Visuals.RUNNING:
                Visual.Animation = "Running";
                break;
            case Visuals.GUNNING:
                Visual.Animation = "Gunning";
                break;
            case Visuals.DODGING:
                Visual.Animation = "Dodge";
                break;
            case Visuals.IDLING:
                Visual.Animation = "Idle";
                break;
        }
        Visual.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        invincibilityFrames--;
        BulletSpawnCountdown--;
        DodgeCountdown--;
        Vector2 direction = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");
        Visual.FlipH = direction.X < 0;

        if (BulletsInChamber == 0)
        {
            ReloadCountdown -= delta;
            if (ReloadCountdown <= 0)
            {
                BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
            }
        }

        // You can't perform any actions in the middle of a dodge
        if (DodgeCountdown > 0)
        {
            Velocity = DodgeDirection * PLAYER_SPEED;
            MoveAndSlide();
            return;
        }
        else if (DodgeCountdown == 0)
        {
            ReloadCountdown = -1; // Always be ready for a ReloadCountdown after a dodge
        }

        if (Input.IsActionPressed("GameDodge") && direction != Vector2.Zero)
        {
            invincibilityFrames = DODGE_I_FRAME_COUNT;
            DodgeCountdown = DODGE_I_FRAME_COUNT;
            DodgeDirection = direction.Normalized();
            SetVisual(Visuals.DODGING);
        }
        else if (Input.IsActionPressed("GameShoot"))
        {
            SetVisual(Visuals.GUNNING);
            if (BulletSpawnCountdown < 0 && direction != new Vector2(0, 0) && BulletsInChamber > 0)
            {
                BulletSpawnCountdown = BULLET_SPAWN_TIME;
                SpawnBullet(direction);
            }
        }
        else
        {
            Velocity = direction * PLAYER_SPEED;
            var collisionHappened = MoveAndSlide();

            if (direction != Vector2.Zero)
            {
                SetVisual(Visuals.RUNNING);
            }
            else
            {
                SetVisual(Visuals.IDLING);
            }

            if (collisionHappened)
            {
                var res = GetLastSlideCollision();
                var collision = (Node2D)res.GetCollider();

                if (collision.IsInGroup("DamagesPlayer") && invincibilityFrames <= 0)
                {
                    HealthPoints--;
                    AdjustHp(HealthPoints);
                }
            }
        }

    }

    private void SpawnBullet(Vector2 direction)
    {
        BulletsInChamber--;
        if (BulletsInChamber == 0)
        {
            ReloadCountdown = RELOAD_TIME;
        }
        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.PLAYER);

        var random = new RandomNumberGenerator();
        random.Randomize();

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-0.05f, 0.05f));
        Level.Spawn(newBullet, Position);
        Level.SpawnParticle(ParticleNames.Explosion, Position);
    }

    private void AdjustHp(int amount)
    {
        invincibilityFrames = I_FRAME_COUNT;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.AdjustHP, amount);
    }
}

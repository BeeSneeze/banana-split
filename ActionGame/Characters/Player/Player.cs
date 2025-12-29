using Godot;
using System;
using Common;

public partial class Player : CharacterBody2D
{
    public Room CurrentRoom;

    private PackedScene BulletScene;
    private AnimatedSprite2D Visual;
    private AnimatedSprite2D Crosshair;

    private const float PLAYER_SPEED = 360.0f;
    private const float BULLET_SPEED = 13.0f;
    private const int I_FRAME_COUNT = 60;
    private const int BULLET_SPAWN_TIME = 9;
    private const double RELOAD_TIME = 1.0;
    private const int MAX_BULLETS_IN_CHAMBER = 6;
    private const int DODGE_I_FRAME_COUNT = 30;

    private double ReloadCountdown = 0;
    private int BulletsInChamber = 6;
    private int invincibilityFrames = 0;
    private int BulletSpawnCountdown = 0;
    private int DodgeCountdown = 0;
    private bool Dodging = false;
    private Vector2 DodgeDirection = new Vector2(0, 0);

    public override void _Ready()
    {
        if (CurrentRoom == null)
        {
            throw new Exception("PLAYER WAS NOT INITIATED!");
        }
        Visual = GetNode<AnimatedSprite2D>("Visual");
        Crosshair = GetNode<AnimatedSprite2D>("Crosshair");
        BulletScene = GD.Load<PackedScene>("res://ActionGame/bullet.tscn");
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

        var normVector = direction.Normalized();
        if (normVector != Vector2.Zero)
        {
            var newTween = GetTree().CreateTween();
            newTween.TweenProperty(Crosshair, "position", new Vector2(normVector.X * 200, normVector.Y * 200), 0.06);
        }

        Visual.FlipH = direction.X < 0;

        if (invincibilityFrames > 0 && !Dodging)
        {
            Visual.Visible = !Visual.Visible;
        }
        else if (invincibilityFrames == 0)
        {
            Visual.Visible = true;
        }

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
            Dodging = false;
            ReloadCountdown = -1; // Always be ready for a ReloadCountdown after a dodge
        }

        if (Input.IsActionPressed("GameDodge") && direction != Vector2.Zero)
        {
            Dodging = true;
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
                // This handles contact damage to enemies
                // Enemy bullets resolve damage themselves
                var collision = GetLastSlideCollision().GetCollider();
                if (collision as PhysicsBody2D != null)
                {
                    if (((PhysicsBody2D)collision).CollisionLayer == (uint)CollisionLayerDefs.ENEMY)
                    {
                        TakeDamage(1);
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
            ReloadCountdown = RELOAD_TIME;
        }
        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.PLAYER);

        var random = new RandomNumberGenerator();
        random.Randomize();

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-0.05f, 0.05f));
        newBullet.Room = CurrentRoom;
        CurrentRoom.Spawn(newBullet, Position);
        CurrentRoom.SpawnParticle(ParticleNames.Dust, Position);
    }

    public void TakeDamage(int amount)
    {
        if (invincibilityFrames > 0)
        {
            return;
        }

        invincibilityFrames = I_FRAME_COUNT;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PlayerTookDamage, amount);
    }
}

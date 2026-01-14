using Godot;
using System;
using Common;

public partial class Player : CharacterBody2D
{
    public Room CurrentRoom;

    private PackedScene BulletScene;
    private AnimatedSprite2D Visual;
    private AnimatedSprite2D Crosshair;

    private const float PLAYER_SPEED = 350.0f;
    private const float BULLET_SPEED = 13.0f;
    private const int I_FRAME_COUNT = 60;
    private const int BULLET_SPAWN_TIME = 10;
    private const double RELOAD_TIME = 1.0;
    private const int MAX_BULLETS_IN_CHAMBER = 6;
    private const int DODGE_FRAME_COUNT = 35;
    private const float DODGE_SPEED_BONUS = 50;
    private const int DODGE_BUFFER_FRAMES = 15;

    private int BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
    private float TemporarySpeed;
    private double ReloadCountdown;
    private int TemporaryBulletSpawnReduction, BulletSpawnCountdown;
    private int InvincibilityFrames;
    private int DodgeCountdown, DodgeBuffer;
    private State ActiveState = State.IDLE;
    private Vector2 DodgeDirection = new Vector2(0, 0);
    private uint normalCollisions = (uint)CollisionLayerDefs.WALLS + (uint)CollisionLayerDefs.ENEMY + (uint)CollisionLayerDefs.OBSTACLES;

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

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");
        var normVector = direction.Normalized();
        if (normVector != Vector2.Zero)
        {
            var newTween = GetTree().CreateTween();
            newTween.TweenProperty(Crosshair, "position", new Vector2(normVector.X * 200, normVector.Y * 200), 0.06);
        }

        // Visuals
        Visual.FlipH = direction.X < 0;
        if (InvincibilityFrames > 0)
        {
            InvincibilityFrames--;
            if (DodgeCountdown <= 0)
            {
                Visual.Visible = !Visual.Visible;
            }
        }
        else if (InvincibilityFrames == 0)
        {
            Visual.Visible = true;
        }

        // Reloading
        if (BulletsInChamber == 0)
        {
            ReloadCountdown -= delta;
            if (ReloadCountdown <= 0)
            {
                BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
            }
        }

        // Dodge mechanics
        DodgeCountdown--;
        if (DodgeCountdown > 0)
        {
            Velocity = DodgeDirection * (PLAYER_SPEED + TemporarySpeed);
            MoveAndSlide();
            return; // You can't perform any actions in the middle of a dodge
        }
        else if (DodgeCountdown == 0)
        {
            ReloadCountdown = -1; // Always reload after a dodge roll
            BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
            CollisionMask = normalCollisions + (uint)CollisionLayerDefs.ENEMY_BULLETS;
            TemporarySpeed = 0;
            DodgeBuffer = DODGE_BUFFER_FRAMES;
        }

        // Button input
        DodgeBuffer--;
        BulletSpawnCountdown--;
        if (Input.IsActionPressed("GameDodge") && direction != Vector2.Zero && DodgeBuffer < 0 && ActiveState != State.SLIDE)
        {
            DodgeDirection = direction.Normalized();
            SetState(State.DODGE);
        }
        else if (Input.IsActionPressed("GameShoot"))
        {
            Shoot(direction);
        }
        else
        {
            Velocity = direction * (PLAYER_SPEED + TemporarySpeed);
            var newState = (direction != Vector2.Zero) ? State.RUNNING : State.IDLE;
            SetState(newState);
        }

        // Player damage
        // TODO: The player can completely avoid damage by standing still
        if (MoveAndCollide(Velocity * (float)delta, true) != null)
        {
            // Generally speaking, the player resolves collisions rather than other entities
            var collision = MoveAndCollide(Velocity, true).GetCollider();
            if (collision as PhysicsBody2D != null)
            {
                if (((PhysicsBody2D)collision).CollisionLayer == (uint)CollisionLayerDefs.ENEMY)
                {
                    TakeDamage(1);
                }
                if (((PhysicsBody2D)collision).CollisionLayer == (uint)CollisionLayerDefs.ENEMY_BULLETS)
                {
                    ((Bullet)collision).ExplodeBullet();
                    TakeDamage(1);
                }
            }
        }

        MoveAndSlide();
    }

    private void Shoot(Vector2 direction)
    {
        if (ActiveState == State.SLIDE)
        {
            Velocity *= 0.97f;
            if (Velocity.Length() < 50.0f)
            {
                SetState(State.RUNNING);
            }
        }
        else if (ActiveState == State.DODGE && direction != new Vector2(0, 0) && Input.IsActionPressed("GameDodge"))
        {
            SetState(State.SLIDE);
        }
        else if (ActiveState != State.GUNNING)
        {
            SetState(State.GUNNING);
        }

        if (BulletSpawnCountdown < 0 && BulletsInChamber > 0)
        {
            if (direction != new Vector2(0, 0))
            {
                BulletSpawnCountdown = BULLET_SPAWN_TIME - TemporaryBulletSpawnReduction;
                SpawnBullet(direction);
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

        Input.StartJoyVibration(0, 0.2f, 0.2f, 0.06f);

        var newBullet = BulletScene.Instantiate<Bullet>();
        newBullet.SetTeam(Team.PLAYER);

        var random = new RandomNumberGenerator();
        random.Randomize();

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-0.09f, 0.09f));
        newBullet.Room = CurrentRoom;
        CurrentRoom.Spawn(newBullet, Position);
        CurrentRoom.SpawnParticle(ParticleNames.Dust, Position);
    }

    private void TakeDamage(int amount)
    {
        if (InvincibilityFrames > 0)
        {
            return;
        }
        Input.StartJoyVibration(0, 0.9f, 0.9f, 0.4f);

        InvincibilityFrames = I_FRAME_COUNT;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PlayerTookDamage, amount);
    }

    private enum State
    {
        RUNNING,
        GUNNING,
        DODGE,
        IDLE,
        SLIDE
    }

    private void SetState(State state)
    {
        ActiveState = state;
        switch (state)
        {
            case State.RUNNING:
                TemporaryBulletSpawnReduction = 0;
                Visual.Animation = "Running";
                break;
            case State.GUNNING:
                Velocity = Vector2.Zero;
                var newTween = GetTree().CreateTween();
                Visual.Scale = new Vector2(0.35f, 0.29f);
                newTween.TweenProperty(Visual, "scale", new Vector2(0.35f, 0.35f), 0.06);
                Visual.Animation = "Gunning";
                break;
            case State.DODGE:
                TemporarySpeed += DODGE_SPEED_BONUS;
                InvincibilityFrames = DODGE_FRAME_COUNT;
                DodgeCountdown = DODGE_FRAME_COUNT;
                CollisionMask = normalCollisions;
                Visual.Animation = "Dodge";
                break;
            case State.IDLE:
                Visual.Animation = "Idle";
                break;
            case State.SLIDE:
                Velocity *= 1.7f;
                TemporaryBulletSpawnReduction = 2;
                Visual.Animation = "Slide";
                break;
        }
        Visual.Play();
    }
}

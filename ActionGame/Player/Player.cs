using Godot;
using System;
using Common;
using System.Security.AccessControl;
using Microsoft.VisualBasic;

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
    private const int DODGE_BUFFER_FRAMES = 20;
    private const float BULLET_SPREAD = 0.07f;

    private int BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
    private float TemporarySpeed;
    private double ReloadCountdown;
    private int TemporaryBulletSpawnReduction, BulletSpawnCountdown;
    private int InvincibilityFrames;
    private int DodgeCountdown, DodgeBuffer;
    private State ActiveState = State.IDLE;
    private Vector2 DodgeDirection = new Vector2(0, 0);
    private Area2D AutoAimArea;
    private const uint COLLISIONS_NORMAL = (uint)CollisionLayerDefs.WALLS + (uint)CollisionLayerDefs.ENEMY + (uint)CollisionLayerDefs.OBSTACLES;
    private const uint COLLISIONS_NO_DODGE = (uint)CollisionLayerDefs.ENEMY_BULLETS + (uint)CollisionLayerDefs.DODGEABLE;

    public Vector2 GetPositionRelativeToRoom()
    {
        return GlobalPosition - CurrentRoom.GlobalPosition;
    }

    public override void _Ready()
    {
        if (CurrentRoom == null)
        {
            throw new Exception("PLAYER WAS NOT INITIATED!");
        }
        Visual = GetNode<AnimatedSprite2D>("Visual");
        Crosshair = GetNode<AnimatedSprite2D>("Crosshair");
        BulletScene = GD.Load<PackedScene>("res://ActionGame/bullet.tscn");
        AutoAimArea = GetNode<Area2D>("AutoAimArea");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");
        Vector2 direction = ApplyAutoAim(inputDirection);

        if (inputDirection != Vector2.Zero)
        {
            var normVector = inputDirection.Normalized();
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
            CollisionMask = COLLISIONS_NORMAL + COLLISIONS_NO_DODGE;
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
            if (collision is PhysicsBody2D body)
            {
                if (body.CollisionLayer == (uint)CollisionLayerDefs.ENEMY)
                {
                    TakeDamage(1);
                }
                if (body.CollisionLayer == (uint)CollisionLayerDefs.ENEMY_BULLETS)
                {
                    ((Bullet)body).ExplodeBullet();
                    TakeDamage(1);
                }
            }
        }

        MoveAndSlide();
    }

    private Vector2 ApplyAutoAim(Vector2 inputVector)
    {
        AutoAimArea.Rotation = Mathf.Atan2(inputVector.Y, inputVector.X);

        var shortestDistance = 100000000.0;
        Node2D closestBody = null;

        foreach (var body in AutoAimArea.GetOverlappingBodies())
        {
            if ((body.GlobalPosition - GlobalPosition).Length() < shortestDistance)
            {
                shortestDistance = (body.GlobalPosition - GlobalPosition).Length();
                closestBody = body;
            }
        }

        if (closestBody != null)
        {
            return (closestBody.GlobalPosition - GlobalPosition).Normalized() * inputVector.Length();
        }

        return inputVector;
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

        newBullet.Velocity = BULLET_SPEED * direction.Normalized().Rotated(random.RandfRange(-BULLET_SPREAD, BULLET_SPREAD));
        newBullet.Room = CurrentRoom;
        CurrentRoom.Spawn(newBullet, GlobalPosition - CurrentRoom.GlobalPosition);
        CurrentRoom.SpawnParticle(ParticleNames.Dust, GlobalPosition - CurrentRoom.GlobalPosition);
    }

    public void TakeDamage(int amount)
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
                CollisionMask = COLLISIONS_NORMAL;
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

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

    private float TemporarySpeed = 0;
    private double ReloadCountdown = 0;
    private int BulletsInChamber = MAX_BULLETS_IN_CHAMBER;
    private int invincibilityFrames = 0;
    private int BulletSpawnCountdown = 0;
    private int DodgeCountdown = 0;
    private State ActiveState = State.IDLING;
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

        if (invincibilityFrames > 0 && DodgeCountdown <= 0)
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
        }

        if (Input.IsActionPressed("GameDodge") && direction != Vector2.Zero)
        {
            invincibilityFrames = DODGE_I_FRAME_COUNT;
            DodgeCountdown = DODGE_I_FRAME_COUNT;
            DodgeDirection = direction.Normalized();
            CollisionMask = normalCollisions;
            TemporarySpeed = 100;
            SetState(State.DODGING);
        }
        else if (Input.IsActionPressed("GameShoot"))
        {
            if (ActiveState == State.RUNNING)
            {
                Velocity = Vector2.Zero;
            }
            SetState(State.GUNNING);
            if (BulletSpawnCountdown < 0 && direction != new Vector2(0, 0) && BulletsInChamber > 0)
            {
                BulletSpawnCountdown = BULLET_SPAWN_TIME;
                SpawnBullet(direction);
            }
        }
        else
        {
            Velocity = direction * (PLAYER_SPEED + TemporarySpeed);

            if (direction != Vector2.Zero)
            {
                SetState(State.RUNNING);
            }
            else
            {
                SetState(State.IDLING);
            }
        }


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

    private void SpawnBullet(Vector2 direction)
    {
        BulletsInChamber--;
        if (BulletsInChamber == 0)
        {
            ReloadCountdown = RELOAD_TIME;
        }

        //Input.StartJoyVibration(0, 0.2f, 0.2f, 0.06f);

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
        if (invincibilityFrames > 0)
        {
            return;
        }

        //Input.StartJoyVibration(0, 0.9f, 0.9f, 0.4f);

        invincibilityFrames = I_FRAME_COUNT;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PlayerTookDamage, amount);
    }

    private enum State
    {
        RUNNING,
        GUNNING,
        DODGING,
        IDLING
    }

    private void SetState(State state)
    {
        ActiveState = state;
        switch (state)
        {
            case State.RUNNING:
                Visual.Animation = "Running";
                break;
            case State.GUNNING:
                var newTween = GetTree().CreateTween();
                Visual.Scale = new Vector2(0.35f, 0.29f);
                newTween.TweenProperty(Visual, "scale", new Vector2(0.35f, 0.35f), 0.06);
                Visual.Animation = "Gunning";
                break;
            case State.DODGING:
                Visual.Animation = "Dodge";
                break;
            case State.IDLING:
                Visual.Animation = "Idle";
                break;
        }
        Visual.Play();
    }
}

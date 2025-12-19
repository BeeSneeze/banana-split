using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    private Game Game;

    private int invincibilityFrames = 0;
    private const int I_FRAME_COUNT = 100;

    private int HealthPoints = 15;

    public override void _Ready()
    {
        // TODO: Fix this reference to the game
        Game = GetParent().GetParent<Game>();
    }

    public const float Speed = 300.0f;
    public override void _PhysicsProcess(double delta)
    {
        invincibilityFrames--;
        Vector2 direction = Input.GetVector("GameLeft", "GameRight", "GameUp", "GameDown");
        Velocity = direction * Speed;
        var collisionHappened = MoveAndSlide();

        if(collisionHappened)
        {
            var res = GetLastSlideCollision();

            var collision = (Node2D)res.GetCollider();
            if(collision.IsInGroup("DamagesPlayer"))
            {
                if(invincibilityFrames < 0)
                {
                    HealthPoints--;
                    AdjustHp(HealthPoints);
                }   
            }
        }
    }

    private void AdjustHp(int amount)
    {
        invincibilityFrames = I_FRAME_COUNT;
        Game.AdjustHp(amount);
    }
}

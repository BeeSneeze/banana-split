using Godot;
using System;

public partial class Particle : AnimatedSprite2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AnimationFinished += () => QueueFree();
        Play();
    }
}

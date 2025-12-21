using Godot;
using System;
using Common;

public partial class ActionGame : Control
{

    private PackedScene ParticleScene;

    public override void _Ready()
    {
        ParticleScene = GD.Load<PackedScene>("res://particle.tscn");
        var playerScene = GD.Load<PackedScene>("res://player.tscn").Instantiate<Player>();
        playerScene.Initiate(this);
        playerScene.Position = new Vector2(200, 200);
        AddChild(playerScene);
    }

    public void Spawn(Node2D levelObject, Vector2 pos)
    {
        levelObject.Position = pos;
        AddChild(levelObject);
    }

    public void SpawnParticle(ParticleNames particleName, Vector2 pos)
    {
        var newParticle = ParticleScene.Instantiate<AnimatedSprite2D>();
        newParticle.Position = pos;

        switch (particleName)
        {
            case ParticleNames.Explosion:
                newParticle.Animation = "Explosion";
                break;
        }

        AddChild(newParticle);

    }
}

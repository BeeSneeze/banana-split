using Godot;
using System;
using Common;

public partial class ActionGame : Control
{
    private PackedScene ParticleScene;
    private int CurrentRoomID = -1;

    public override void _Ready()
    {
        ParticleScene = GD.Load<PackedScene>("res://particle.tscn");
        var playerScene = GD.Load<PackedScene>("res://ActionGame/Characters/player.tscn").Instantiate<Player>();
        playerScene.Initiate(this);
        playerScene.Position = new Vector2(200, 200);
        GetNode("EntityMap").AddChild(playerScene);

        CustomEvents.Instance.PlayerChangedRoom += ChangeRoom;
    }

    private void ChangeRoom(int roomID)
    {
        CurrentRoomID = roomID;
        GD.Print("Changed to room: " + CurrentRoomID.ToString());
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
            case ParticleNames.Dust:
                newParticle.Animation = "Dust";
                break;
        }

        AddChild(newParticle);
    }
}

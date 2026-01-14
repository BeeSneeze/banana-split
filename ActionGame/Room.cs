using Godot;
using System;
using Common;

public partial class Room : Node2D
{
    [Export]
    public int RoomID;

    private PackedScene ParticleScene;
    private bool Initiated = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ParticleScene = GD.Load<PackedScene>("res://ActionGame/Particles/particle.tscn");
        CustomEvents.Instance.PlayerChangedRoom += PlayerChangedRoom;
    }

    public override void _Process(double delta)
    {
        // Tilemap entities are sadly initiated *after* the ready function
        if (GetChildCount() > 1 && !Initiated)
        {
            SetRoomReference();
        }
    }

    public void DeactivateRoom()
    {
        Modulate = new Color(0.8f, 0.8f, 0.8f);
        foreach (var entity in GetNode("EntityMap").GetChildren())
        {
            if (entity as RoomExitArea != null)
            {
                ((RoomExitArea)entity).OnPlayerLeave();
            }
        }
        GetNode("EntityMap").QueueFree();
    }

    private void PlayerChangedRoom(int roomID)
    {
        if (roomID == RoomID)
        {
            foreach (var entity in GetNode("EntityMap").GetChildren())
            {
                if (entity as Enemy != null)
                {
                    ((Enemy)entity).SetPhysicsProcess(true);
                }
            }
        }
    }

    private void SetRoomReference()
    {
        foreach (var entity in GetNode("EntityMap").GetChildren())
        {
            if (entity as RoomEntranceArea != null)
            {
                ((RoomEntranceArea)entity).Room = this;
            }
            if (entity as RoomExitArea != null)
            {
                ((RoomExitArea)entity).Room = this;
            }
        }
        Initiated = true;
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
            case ParticleNames.Splash:
                newParticle.Animation = "Splash";
                break;
        }

        AddChild(newParticle);
    }

}

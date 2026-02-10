using Godot;
using System;
using Common;
using System.Linq;

public partial class Room : Node2D
{
    [Export]
    public int RoomID;

    private PackedScene ParticleScene;
    private bool Initiated = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<CanvasItem>("EntityMap").Visible = false;
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
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0.0f), 0.2);
        GetNode("EntityMap").QueueFree();
        tween.Finished += () => QueueFree();
    }

    private void PlayerChangedRoom(int roomID)
    {
        if (roomID == RoomID)
        {
            GetNode<CanvasItem>("EntityMap").Visible = true;

            foreach (var entity in GetNode("EntityMap").GetChildren())
            {
                if (entity is Enemy enemy)
                {
                    enemy.SetPhysicsProcess(true);
                }
                if (entity is RoomExitArea roomExit)
                {
                    roomExit.OnPlayerLeave();
                }
            }

        }
    }

    private void SetRoomReference()
    {
        foreach (var entity in GetNode("EntityMap").GetChildren())
        {
            if (entity is RoomEntranceArea roomEntrance)
            {
                roomEntrance.Room = this;
            }
            if (entity is RoomExitArea roomExit)
            {
                roomExit.Room = this;
            }
            if (entity is RoomDoor roomDoor)
            {
                roomDoor.Room = this;
            }
        }
        Initiated = true;
    }

    public int GetEnemyCount()
    {
        return GetNode("EntityMap").GetChildren().Where(x => x is Enemy).ToArray().Length;
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
        newParticle.Animation = particleName.ToString();

        AddChild(newParticle);
    }

}

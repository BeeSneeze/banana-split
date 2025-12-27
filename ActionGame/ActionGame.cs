using Godot;
using System;
using Common;

public partial class ActionGame : Control
{
    private int CurrentRoomID = -1;
    private Room CurrentRoom;
    private Player Player;

    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://ActionGame/Characters/player.tscn").Instantiate<Player>();
        Player.Initiate(this);
        Player.Position = new Vector2(0, 0);
        CurrentRoom = GetChild<Room>(0);
        CurrentRoom.GetNode("EntityMap").AddChild(Player);

        CustomEvents.Instance.PlayerChangedRoom += ChangeRoom;
    }

    private void ChangeRoom(int roomID)
    {
        CurrentRoomID = roomID;

        foreach (Room room in GetChildren())
        {
            if (room.RoomID == CurrentRoomID)
            {
                CurrentRoom = room;
            }
        }

        GD.Print("Changed to room: " + CurrentRoomID.ToString());
    }

    public void Spawn(Node2D levelObject, Vector2 pos)
    {
        CurrentRoom.Spawn(levelObject, pos);
    }

    public void SpawnParticle(ParticleNames particleName, Vector2 pos)
    {
        CurrentRoom.SpawnParticle(particleName, pos);
    }

}

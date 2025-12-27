using Godot;
using System;
using Common;
using System.Net.Sockets;

public partial class ActionGame : Control
{
    private int CurrentRoomID = -1;
    private Room CurrentRoom;
    private Player Player;
    private bool RecentReparent = false;


    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://ActionGame/Characters/player.tscn").Instantiate<Player>();
        Player.Initiate(this);
        Player.Position = new Vector2(0, 0);
        CurrentRoom = GetChild<Room>(0);
        CurrentRoom.AddChild(Player);

        CustomEvents.Instance.PlayerChangedRoom += ChangeRoom;
    }

    public override void _PhysicsProcess(double delta)
    {
        // This check is needed or else the game crashes
        if (RecentReparent)
        {
            RecentReparent = false;
        }
    }


    private void ChangeRoom(int roomID)
    {
        CurrentRoomID = roomID;

        foreach (var child in GetChildren())
        {
            if (child as Room != null)
            {
                var room = (Room)child;
                if (room.RoomID == CurrentRoomID)
                {
                    CurrentRoom = room;
                }
            }

        }

        GD.Print("Changed to room: " + CurrentRoomID.ToString());
        if (!RecentReparent && Player.GetParent() != CurrentRoom)
        {
            Player.CallDeferred(Node2D.MethodName.Reparent, CurrentRoom);
            RecentReparent = true;
        }
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

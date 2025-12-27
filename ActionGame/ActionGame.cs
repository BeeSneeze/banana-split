using Godot;
using System;
using Common;
using System.Net.Sockets;
using System.Linq;

public partial class ActionGame : Control
{
    private int CurrentRoomID = -1;
    private Room CurrentRoom;
    private Player Player;
    private bool RecentReparent = false;


    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://ActionGame/Characters/player.tscn").Instantiate<Player>();
        Player.Position = new Vector2(0, 0);
        CurrentRoom = GetChild<Room>(0);
        Player.CurrentRoom = CurrentRoom; // NOTE: Player needs to have current room before being added to the scene!
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
        CurrentRoom = (Room)GetChildren().First(x => ((Room)x).RoomID == roomID);

        GD.Print("Changed to room: " + CurrentRoomID.ToString());
        if (!RecentReparent && Player.GetParent() != CurrentRoom)
        {
            // Defer and delay to avoid crashing
            Player.CallDeferred(Node2D.MethodName.Reparent, CurrentRoom);
            RecentReparent = true;
            Player.CurrentRoom = CurrentRoom;
        }
    }

}

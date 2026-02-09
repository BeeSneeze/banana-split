using Godot;
using System;

public partial class RoomDoor : StaticBody2D
{
    public Room Room;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CustomEvents.Instance.PlayerClearedRoom += PlayerClearedRoom;
    }

    private void PlayerClearedRoom(int roomID)
    {
        if (Room.RoomID == roomID)
        {
            QueueFree();
        }
    }
}

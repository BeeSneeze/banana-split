using Godot;
using System;

public partial class RoomEntranceArea : Area2D
{
    public Room Room;

    public void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PlayerChangedRoom, Room.RoomID);
        }
    }
}

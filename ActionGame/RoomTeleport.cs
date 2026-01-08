using Godot;
using System;

public partial class RoomTeleport : Area2D
{
    public int RoomID;

    public void OnBodyEntered(Node2D body)
    {
        if (body as Player != null)
        {
            CustomEvents.Instance.EmitSignal("PlayerChangedRoom", RoomID);
        }
    }
}

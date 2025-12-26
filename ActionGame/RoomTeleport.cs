using Godot;
using System;

public partial class RoomTeleport : Area2D
{
    [Export]
    public int RoomID;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body as Player != null)
        {
            CustomEvents.Instance.EmitSignal("PlayerChangedRoom", RoomID);
        }
    }
}

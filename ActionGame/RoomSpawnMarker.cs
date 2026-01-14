using Godot;
using System;

public partial class RoomSpawnMarker : Sprite2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Modulate = new Color(1, 1, 1, 0);
    }
}

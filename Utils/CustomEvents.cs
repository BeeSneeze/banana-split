using Common;
using Godot;
using System;

public partial class CustomEvents : Node
{
    [Signal]
    public delegate void AdjustHPEventHandler(int HP);
    [Signal]
    public delegate void PlayerChangedRoomEventHandler(int roomID);

    public static CustomEvents Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}

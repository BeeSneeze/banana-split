using Common;
using Godot;
using System;

public partial class CustomEvents : Node
{
    [Signal]
    public delegate void PlayerTookDamageEventHandler(int HP);
    [Signal]
    public delegate void PlayerChangedRoomEventHandler(int roomID);
    [Signal]
    public delegate void GameOverEventHandler();
    [Signal]
    public delegate void GameWonEventHandler();



    public static CustomEvents Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}

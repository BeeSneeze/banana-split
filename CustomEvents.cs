using Godot;
using System;

public partial class CustomEvents : Node
{
    public static CustomEvents Instance { get; private set; }

    [Signal]
    public delegate void AdjustHPEventHandler(int HP);

    // TODO: How to fix things if I change the name of an event?
    //public static readonly string ADJUST_HP = "AdjustHP";

    public override void _Ready()
    {
        Instance = this;
    }
}

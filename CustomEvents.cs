using Common;
using Godot;
using System;

public partial class CustomEvents : Node
{
    public static CustomEvents Instance { get; private set; }

    [Signal]
    public delegate void AdjustHPEventHandler(int HP);

    public override void _Ready()
    {
        Instance = this;
    }
}

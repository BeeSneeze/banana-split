using Godot;
using System;

public partial class Flower : Area2D
{
    public void OnBodyEnter(Node2D body)
    {
        if (body is Player)
        {
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.GameWon);
        }
    }
}

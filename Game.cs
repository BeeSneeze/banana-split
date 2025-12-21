using Godot;
using System;

public partial class Game : Control
{
    private Label HP;

    public override void _Ready()
    {
        HP = GetNode<Label>("%HP");
    }

    public void AdjustHp(int amount)
    {
        GD.Print("Damage taken: " + amount.ToString());
        HP.Text = amount.ToString();
    }
}

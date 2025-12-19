using Godot;
using System;

public partial class Game : Control
{
    private Label HP;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HP = GetNode<Label>("%HP");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }

    public void AdjustHp(int amount)
    {
        GD.Print("Damage taken: " + amount.ToString());
        HP.Text = amount.ToString();
    }
}

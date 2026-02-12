using Common;
using Godot;

public partial class PowerupItem : AnimatedSprite2D
{
    public Powerup Powerup = Powerup.STICK;
    private Node2D ToolTip;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ToolTip = GetNode<Node2D>("ToolTip");
        ToolTip.Visible = false;
        ToolTip.GetNode<Label>("Title").Text = Powerup.Name;
        ToolTip.GetNode<RichTextLabel>("Label").Text = Powerup.Label;
    }

    public void ToggleToolTip()
    {
        ToolTip.Visible = !ToolTip.Visible;
    }
}
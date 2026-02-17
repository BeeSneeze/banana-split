using Common;
using Godot;
using Powerups;

public partial class PowerupItem : Node2D
{
    public Powerup Powerup = Powerup.STICK;
    private Node2D ToolTip;

    public override void _Ready()
    {
        ToolTip = GetNode<Node2D>("ToolTip");
        ToolTip.Visible = false;
        ToolTip.GetNode<Label>("Title").Text = Powerup.Name;
        ToolTip.GetNode<RichTextLabel>("Label").Text = Powerup.FlavorText;
        GetNode<AnimatedSprite2D>("Sprite").Animation = Powerup.ToString();
    }

    public void ToggleToolTip()
    {
        ToolTip.Visible = !ToolTip.Visible;
    }
}
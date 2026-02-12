using Godot;
using System;
using System.Xml.Schema;

public partial class PowerupShop : Node2D
{
    bool ToolTipsVisible = false;

    PowerupItem LeftItem, RightItem;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Position += new Vector2(0, -30);
        var scene = GD.Load<PackedScene>("res://ActionGame/powerup_item.tscn");
        LeftItem = scene.Instantiate<PowerupItem>();
        LeftItem.Position = new Vector2(-100, 0);
        AddChild(LeftItem);
        RightItem = scene.Instantiate<PowerupItem>();
        AddChild(RightItem);
    }

    private void ClearPowerup(string _pow)
    {
        this.QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("GameShoot") && ToolTipsVisible)
        {
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PowerupCollected, LeftItem.Powerup.Name);
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PowerupCollected, RightItem.Powerup.Name);
        }
    }

    public void PlayerEnterOrExit(Node2D body)
    {
        if (body is Player player)
        {
            LeftItem.ToggleToolTip();
            RightItem.ToggleToolTip();
        }
    }
}

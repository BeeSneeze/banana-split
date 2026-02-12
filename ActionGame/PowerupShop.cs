using Godot;
using System;
using System.Xml.Schema;
using Common;

public partial class PowerupShop : Node2D
{
    private bool ToolTipsVisible = false;
    private PowerupItem LeftItem, RightItem;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<CanvasItem>("ToolTip").Visible = ToolTipsVisible;
        Position += new Vector2(0, -20);
        var scene = GD.Load<PackedScene>("res://ActionGame/powerup_item.tscn");
        LeftItem = scene.Instantiate<PowerupItem>();
        LeftItem.Position = new Vector2(-100, 0);
        AddChild(LeftItem);
        RightItem = scene.Instantiate<PowerupItem>();
        AddChild(RightItem);
        CustomEvents.Instance.PowerupCollected += ClearShop;
    }

    private void ClearShop(string _pow)
    {
        CustomEvents.Instance.PowerupCollected -= ClearShop;
        GetNode<Area2D>("Area2D")?.QueueFree();
        LeftItem?.QueueFree();
        ActionGame.CurrentRoom.SpawnParticle(ParticleNames.Explosion, LeftItem.Position + Position);
        RightItem?.QueueFree();
        ActionGame.CurrentRoom.SpawnParticle(ParticleNames.Explosion, RightItem.Position + Position);
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
            ToolTipsVisible = !ToolTipsVisible;
            GetNode<CanvasItem>("ToolTip").Visible = ToolTipsVisible;
            LeftItem.ToggleToolTip();
            RightItem.ToggleToolTip();
        }
    }
}

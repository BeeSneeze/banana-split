using Godot;
using Common;
using Powerups;

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
        LeftItem.GetNode<Node2D>("ToolTip").Position += new Vector2(-55, 0);
        LeftItem.GetNode<AnimatedSprite2D>("ToolTip").Play();
        LeftItem.Powerup = Powerup.AllPowerups[GD.RandRange(0, Powerup.AllPowerups.Length - 1)];
        AddChild(LeftItem);
        RightItem = scene.Instantiate<PowerupItem>();
        RightItem.GetNode<Node2D>("ToolTip").Position += new Vector2(55, 0);
        RightItem.GetNode<AnimatedSprite2D>("ToolTip").Animation = "Right";
        RightItem.GetNode<AnimatedSprite2D>("ToolTip").Play();
        RightItem.Powerup = Powerup.AllPowerups[GD.RandRange(0, Powerup.AllPowerups.Length - 1)];
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
        if (Input.IsActionPressed("BuyItem") && ToolTipsVisible)
        {
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PowerupCollected, LeftItem.Powerup.ToString());
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.PowerupCollected, RightItem.Powerup.ToString());
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

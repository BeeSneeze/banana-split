using Godot;
using System;

public partial class ActionGame : Control
{
    public override void _Ready()
    {
        var playerScene = GD.Load<PackedScene>("res://player.tscn").Instantiate<Player>();
        playerScene.Initiate(GetParent<Game>(), this);
        playerScene.Position = new Vector2(200, 200);
        AddChild(playerScene);
    }

    public void Spawn(Node2D levelObject, Vector2 pos)
    {
        levelObject.Position = pos;
        AddChild(levelObject);
    }
}

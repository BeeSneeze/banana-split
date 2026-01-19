using Godot;
using System;
using Common;

public partial class Npc : StaticBody2D
{
    private Area2D InteractionArea;
    private bool ReadyForDialogue = false;
    private NPCName CharacterName = NPCName.Jenny;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InteractionArea = GetNode<Area2D>("InteractionArea");
        GetNode<AnimatedSprite2D>("Visual").Animation = CharacterName.ToString();
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var body in InteractionArea.GetOverlappingBodies())
        {
            if (body is Player player)
            {
                if (Input.IsActionJustPressed("InteractButton"))
                {
                    CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.DialogueStarted, CharacterName.ToString());
                }
            }
        }


    }

}

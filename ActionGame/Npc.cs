using Godot;
using Common;

public partial class Npc : StaticBody2D
{
    public NPCName CharacterName = NPCName.Jenny;
    private Area2D InteractionArea;
    private bool ReadyForDialogue = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InteractionArea = GetNode<Area2D>("InteractionArea");
        GetNode<AnimatedSprite2D>("Visual").Animation = CharacterName.Name;
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var body in InteractionArea.GetOverlappingBodies())
        {
            if (body is Player player && Input.IsActionJustPressed("InteractButton") && ReadyForDialogue)
            {
                ReadyForDialogue = false;
                CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.DialogueStarted, CharacterName.Name);
            }
        }


    }

}

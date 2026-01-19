using Common;
using Godot;

public partial class Game : Control
{
    public override void _Ready()
    {
        CustomEvents.Instance.DialogueStarted += StartDialogue;
    }

    private void StartDialogue(string npc)
    {
        GetNode<CanvasLayer>("ActionGameLayer").SetPhysicsProcess(false);
        // TODO: Fix constant string stuff for NPC names
        switch (npc)
        {
            case "Jenny":
                GD.Print("Jenny starts talking");
                break;
        }
        EndDialogue();
    }

    private void EndDialogue()
    {
        GetNode<CanvasLayer>("ActionGameLayer").SetPhysicsProcess(true);
    }
}

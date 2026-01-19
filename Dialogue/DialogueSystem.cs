using Common;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueSystem : CanvasLayer
{
    private record struct ConversationLine(NPCName speaker, string text);
    private Queue<ConversationLine> ActiveConversation = new Queue<ConversationLine>();

    public override void _Ready()
    {
        ActiveConversation.Enqueue(new ConversationLine(NPCName.Jenny, "Hello!"));
        ActiveConversation.Enqueue(new ConversationLine(NPCName.Jenny, "Goodbye!"));
        this.Visible = false;
        CustomEvents.Instance.DialogueStarted += StartDialogue;
    }

    private void StartDialogue(string npc)
    {
        this.Visible = true;
        // TODO: Fix constant string stuff for NPC names
        switch (npc)
        {
            case "Jenny":
                GD.Print("Jenny starts talking");
                break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("InteractButton"))
        {
            AdvanceDialogue();
        }
    }


    private void AdvanceDialogue()
    {
        if (ActiveConversation.Count > 0)
        {
            var line = ActiveConversation.Dequeue();
            GD.Print(line.text);
        }

        if (ActiveConversation.Count == 0)
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        GD.Print("Dialogue ended");
        this.Visible = false;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.DialogueEnded);
    }
}

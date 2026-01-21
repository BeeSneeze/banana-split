using Common;
using Godot;
using System.Collections.Generic;

public partial class DialogueSystem : CanvasLayer
{
    private record struct ConversationLine(NPCName speaker, string text);
    private Queue<ConversationLine> ActiveConversation = new Queue<ConversationLine>();
    private PackedScene DialogueTextBox;
    private VBoxContainer TextContainer;

    public override void _Ready()
    {

        TextContainer = GetNode<VBoxContainer>("TextContainer");
        DialogueTextBox = GD.Load<PackedScene>("res://Dialogue/dialogue_text_box.tscn");
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
            var newBox = DialogueTextBox.Instantiate<DialogueTextBox>();
            newBox.Speaker = line.speaker;
            newBox.Text = line.text;
            TextContainer.AddChild(newBox);
        }
        else
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

using Common;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class DialogueSystem : CanvasLayer
{
    private record struct ConversationLine(NPCSpeaker Speaker, string Text);
    private Queue<ConversationLine> ActiveConversation = new Queue<ConversationLine>();
    private PackedScene DialogueTextBox;
    private VBoxContainer TextContainer;
    private int RB_COUNT = 0;
    private bool ConversationActive = false;

    public override void _Ready()
    {
        Visible = false;
        TextContainer = GetNode<VBoxContainer>("TextContainer");
        DialogueTextBox = GD.Load<PackedScene>("res://Dialogue/dialogue_text_box.tscn");
        CustomEvents.Instance.DialogueStarted += StartDialogue;
        //LoadConversation("RB/1");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("InteractButton") && ConversationActive)
        {
            AdvanceDialogue();
        }
    }

    private string LoadFromFile(string url)
    {
        var file = FileAccess.Open(url, FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        return content;
    }

    private record struct Conversation(NPCSpeaker[] Speakers, ConversationLine[] Lines);

    private void LoadConversation(string DialogueName)
    {
        var jsonObject = new Json();
        var parsedJson = jsonObject.Parse(LoadFromFile("res://Dialogue/" + DialogueName + ".JSON"));

        if (parsedJson == Error.Ok)
        {
            var conversation = JsonSerializer.Deserialize<Conversation>(Json.Stringify(jsonObject.Data));

            foreach (var line in conversation.Lines)
            {
                ActiveConversation.Enqueue(line);
            }
        }
        else
        {
            throw new System.Exception("Json failed to load");
        }
    }

    private void StartDialogue(string npc)
    {
        if (ConversationActive)
        {
            return; // Never interrupt an already ongoing conversation
        }

        ConversationActive = true;

        this.Visible = true;
        switch (npc)
        {
            case NPCSpeaker.RB:
                RB_COUNT++;
                LoadConversation("RB/" + RB_COUNT.ToString());
                break;
            case NPCSpeaker.LIZ:
                RB_COUNT++;
                LoadConversation("RB/" + RB_COUNT.ToString());
                break;
        }

    }

    private void AdvanceDialogue()
    {
        if (ActiveConversation.Count > 0)
        {
            var line = ActiveConversation.Dequeue();
            var newBox = DialogueTextBox.Instantiate<DialogueTextBox>();
            newBox.Speaker = line.Speaker;
            newBox.Text = line.Text;
            TextContainer.AddChild(newBox);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        ConversationActive = false;
        this.Visible = false;
        CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.DialogueEnded);

        var allChildren = TextContainer.GetChildren();
        for (int i = 0; i < allChildren.Count; i++)
        {
            allChildren[i].QueueFree();
        }
    }
}

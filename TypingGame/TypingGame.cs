using Godot;
using System;
using Common;
using System.Collections.Generic;
using System.Linq;

public partial class TypingGame : CanvasLayer
{
    private Queue<TextBox> ActiveBoxes = new Queue<TextBox>();

    private string HeldString = "";
    private int CharacterIndex = 0;

    private string[] WordList = { "banana", "apple", "acorn", "fudge", "bee" };

    public override void _Ready()
    {
        var TextBoxScene = GD.Load<PackedScene>("res://TypingGame/TextBox.tscn");

        for (int i = 0; i < 5; i++)
        {
            var newBox = TextBoxScene.Instantiate<TextBox>();
            newBox.Initialize(WordList[i], TextBoxType.Normal);
            GetNode("TypingScreen").AddChild(newBox);
            ActiveBoxes.Enqueue(newBox);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed)
            {
                if (ActiveBoxes.First().TypeText[CharacterIndex].ToString() == eventKey.AsTextKeycode())
                {
                    HeldString += eventKey.AsTextKeycode();
                    GD.Print(HeldString);
                    CharacterIndex++;
                    if (CharacterIndex == ActiveBoxes.First().TypeText.Length)
                    {
                        GD.Print("WORD FINISHED!");
                        var finishedBox = ActiveBoxes.Dequeue();
                        finishedBox.QueueFree();
                        CharacterIndex = 0;
                        HeldString = "";
                    }
                }
                else
                {
                    HeldString = "";
                    CharacterIndex = 0;
                }
            }
        }
    }

}

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
    private string[] WordList;
    private PackedScene TextBoxScene;

    private string LoadFromFile(string url)
    {
        using var file = FileAccess.Open(url, FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        return content;
    }

    public override void _Ready()
    {
        WordList = LoadFromFile("res://TypingGame/LeftWords.txt").Split(" ");
        TextBoxScene = GD.Load<PackedScene>("res://TypingGame/TextBox.tscn");

        for (int i = 0; i < 3; i++)
        {
            CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1));
        }
    }

    private void CreateNewActiveBox(int wordIndex)
    {
        var newBox = TextBoxScene.Instantiate<TextBox>();
        newBox.Initialize(WordList[wordIndex], TextBoxType.Normal);

        var TypeScreen = GetNode("TypingScreen");

        TypeScreen.AddChild(newBox);
        TypeScreen.MoveChild(newBox, 0);
        ActiveBoxes.Enqueue(newBox);

        float intensity = 1f;
        foreach (var box in ActiveBoxes)
        {
            box.SetIntensity(intensity);
            intensity -= 0.2f;
        }
    }

    private void ResetActiveBox()
    {
        CharacterIndex = 0;
        HeldString = "";
    }

    private void FinishBox()
    {
        GD.Print("WORD FINISHED!");
        var finishedBox = ActiveBoxes.Dequeue();
        finishedBox.QueueFree();
        ResetActiveBox();
        CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (ActiveBoxes.First().TypeText[CharacterIndex].ToString() == eventKey.AsTextKeycode())
            {
                HeldString += eventKey.AsTextKeycode();
                GD.Print(HeldString);
                CharacterIndex++;
                if (CharacterIndex == ActiveBoxes.First().TypeText.Length)
                {
                    FinishBox();
                }
            }
        }
    }

}

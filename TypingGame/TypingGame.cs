using Godot;
using System;
using Common;
using System.Collections.Generic;
using System.Linq;

public partial class TypingGame : CanvasLayer
{
    private Queue<InventoryBox> InventoryBoxes = new Queue<InventoryBox>(); // The boxes at the top of the screen
    private Queue<TextBox> ActiveBoxes = new Queue<TextBox>(); // The boxes in the minigame area
    private string HeldString = "";
    private int CharacterIndex = 0;
    private string[] WordList;
    private PackedScene TextBoxScene;
    private PackedScene InventoryBoxScene;

    private string LoadFromFile(string url)
    {
        var file = FileAccess.Open(url, FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        return content;
    }

    public override void _Ready()
    {
        CustomEvents.Instance.PlayerTookDamage += AddBoxToInventory;
        WordList = LoadFromFile("res://TypingGame/LeftWords.txt").Split(" ");
        TextBoxScene = GD.Load<PackedScene>("res://TypingGame/TextBox.tscn");
        InventoryBoxScene = GD.Load<PackedScene>("res://TypingGame/queue_box.tscn");

        for (int i = 0; i < 3; i++)
        {
            CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1));
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (ActiveBoxes.Count == 0)
        {
            return;
        }
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (ActiveBoxes.First().TypeText[CharacterIndex].ToString() == eventKey.AsTextKeycode())
            {
                HeldString += eventKey.AsTextKeycode();
                ActiveBoxes.First().SetHighlight(HeldString);
                GD.Print(HeldString);
                CharacterIndex++;
                if (CharacterIndex == ActiveBoxes.First().TypeText.Length)
                {
                    FinishBox();
                }
            }
            else
            {
                ActiveBoxes.First().WrongAnswer();
            }
        }
    }

    private void AddBoxToInventory(int amount)
    {
        GD.Print("Took damage, adding box to inventory");
        var InventoryBar = GetNode("InventoryBar");
        var newBox = InventoryBoxScene.Instantiate<InventoryBox>();
        InventoryBar.AddChild(newBox);
        InventoryBar.MoveChild(newBox, 0);
        InventoryBoxes.Enqueue(newBox);

        if (InventoryBoxes.Count > 7)
        {
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.GameOver);
        }

        if (ActiveBoxes.Count < 3)
        {
            RemoveBoxFromInventory();
        }
    }

    private void RemoveBoxFromInventory()
    {
        var inventoryBox = InventoryBoxes.Dequeue();
        inventoryBox.QueueFree();
        CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1));
    }

    private void CreateNewActiveBox(int wordIndex)
    {
        var newBox = TextBoxScene.Instantiate<TextBox>();
        newBox.Initialize(WordList[wordIndex], TextBoxType.Normal);

        var TypeScreen = GetNode("TypingScreen");

        TypeScreen.AddChild(newBox);
        TypeScreen.MoveChild(newBox, 0);
        ActiveBoxes.Enqueue(newBox);
        UpdateIntensity();
    }

    private void UpdateIntensity()
    {
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
        finishedBox.CompleteAnswer();
        ResetActiveBox();
        UpdateIntensity();
        if (InventoryBoxes.Count > 0)
        {
            RemoveBoxFromInventory();
        }
    }

}

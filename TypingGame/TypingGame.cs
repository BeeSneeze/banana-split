using Godot;
using Common;
using System.Collections.Generic;
using System.Linq;

public partial class TypingGame : CanvasLayer
{
    private Queue<InventoryBox> InventoryBoxes = new Queue<InventoryBox>(); // The boxes at the top of the screen
    private Queue<TextBox> ActiveMinigames = new Queue<TextBox>(); // The boxes in the minigame area
    private InventoryBox ActiveBox;
    private CenterContainer ActiveBoxContainer;
    private string HeldString = "";
    private int CharacterIndex;
    private string[] WordList;
    private PackedScene TextBoxScene, InventoryBoxScene;
    private AnimatedSprite2D WorkerAnimation;

    public override void _Ready()
    {
        ActiveBoxContainer = GetNode<CenterContainer>("ActiveBoxContainer");
        WorkerAnimation = GetNode<AnimatedSprite2D>("Worker");
        WorkerAnimation.AnimationFinished += () => WorkerAnimation.Animation = "Idle";
        CustomEvents.Instance.PlayerTookDamage += AddBoxToInventory;
        TextBoxScene = GD.Load<PackedScene>("res://TypingGame/TextBox.tscn");
        InventoryBoxScene = GD.Load<PackedScene>("res://TypingGame/InventoryBox.tscn");
        WordList = FileAccess
            .Open("res://TypingGame/LeftWords.txt", FileAccess.ModeFlags.Read)
            .GetAsText()
            .Split(" ");

        AddBoxToInventory(3, DamageType.Text.ToString());
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && ActiveMinigames.Count > 0)
        {
            if (ActiveMinigames.First().TypeText[CharacterIndex].ToString() == eventKey.AsTextKeycode())
            {
                HeldString += eventKey.AsTextKeycode();
                ActiveMinigames.First().SetHighlight(HeldString);
                ActiveMinigames.First().RightAnswer();
                CharacterIndex++;
                WorkerAnimation.Animation = "Work";
                WorkerAnimation.Play();
                if (CharacterIndex == ActiveMinigames.First().TypeText.Length)
                {
                    FinishMinigame();
                }
            }
            else
            {
                ActiveMinigames.First().WrongAnswer();
            }
        }
    }

    private void AddBoxToInventory(int amount, string damageTypeName)
    {
        GD.Print("Took damage, adding box to inventory");
        var InventoryBar = GetNode("InventoryBar");
        var newBox = InventoryBoxScene.Instantiate<InventoryBox>();
        newBox.Damage = amount;

        switch (damageTypeName)
        {
            case "Text":
                newBox.damageType = DamageType.Text;
                break;
            case "Scramble":
                newBox.damageType = DamageType.Scramble;
                break;
        }

        InventoryBar.AddChild(newBox);
        InventoryBoxes.Enqueue(newBox);

        if (InventoryBoxes.Count > 7)
            CustomEvents.Instance.EmitSignal(CustomEvents.SignalName.GameOver);

        if (ActiveMinigames.Count == 0)
            ActivateInventoryBox(InventoryBoxes.Dequeue());
    }

    private void ActivateInventoryBox(InventoryBox box)
    {
        ActiveBox = box;
        box.Reparent(ActiveBoxContainer);

        for (int i = 0; i < box.Damage; i++)
        {
            var newBox = TextBoxScene.Instantiate<TextBox>();

            switch (box.damageType)
            {
                case DamageType.Text:
                    newBox.Initialize(WordList[GD.RandRange(0, WordList.Length - 1)], DamageType.Text);
                    break;
                case DamageType.Scramble:
                    string[] characterList = ["q", "w", "e", "a", "s", "d", "z", "x", "c"];
                    var newWord = "";
                    for (int k = 0; k < 6; k++)
                    {
                        newWord += characterList[GD.RandRange(0, 8)];
                    }
                    newBox.Initialize(newWord, DamageType.Text);
                    break;
            }

            var TypeScreen = GetNode("TypingScreen");
            TypeScreen.AddChild(newBox);
            TypeScreen.MoveChild(newBox, 0);
            ActiveMinigames.Enqueue(newBox);
            UpdateIntensity();
        }

    }

    private void UpdateIntensity()
    {
        float intensity = 1f;
        foreach (var box in ActiveMinigames)
        {
            box.SetIntensity(intensity);
            intensity -= 0.3f;
        }
    }

    private void FinishMinigame()
    {
        WorkerAnimation.Animation = "NewPackage";
        WorkerAnimation.Play();
        var finishedBox = ActiveMinigames.Dequeue();
        finishedBox.CompleteAnswer();
        CharacterIndex = 0;
        HeldString = "";
        UpdateIntensity();
        if (ActiveMinigames.Count == 0)
        {
            ActiveBox?.QueueFree();
        }

        if (InventoryBoxes.Count > 0 && ActiveMinigames.Count == 0)
            ActivateInventoryBox(InventoryBoxes.Dequeue());
    }

}

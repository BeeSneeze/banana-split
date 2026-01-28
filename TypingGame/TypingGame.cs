using Godot;
using Common;
using System.Collections.Generic;
using System.Linq;

public partial class TypingGame : CanvasLayer
{
    private Queue<InventoryBox> InventoryBoxes = new Queue<InventoryBox>(); // The boxes at the top of the screen
    private Queue<TextBox> ActiveBoxes = new Queue<TextBox>(); // The boxes in the minigame area
    private string HeldString = "";
    private int CharacterIndex;
    private string[] WordList;
    private PackedScene TextBoxScene, InventoryBoxScene;
    private AnimatedSprite2D WorkerAnimation;

    public override void _Ready()
    {
        WorkerAnimation = GetNode<AnimatedSprite2D>("Worker");
        WorkerAnimation.AnimationFinished += () => WorkerAnimation.Animation = "Idle";
        CustomEvents.Instance.PlayerTookDamage += AddBoxToInventory;
        var file = FileAccess.Open("res://TypingGame/LeftWords.txt", FileAccess.ModeFlags.Read);
        WordList = file.GetAsText().Split(" ");
        TextBoxScene = GD.Load<PackedScene>("res://TypingGame/TextBox.tscn");
        InventoryBoxScene = GD.Load<PackedScene>("res://TypingGame/InventoryBox.tscn");

        for (int i = 0; i < 3; i++)
        {
            CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1), DamageType.Text);
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
                ActiveBoxes.First().RightAnswer();
                CharacterIndex++;
                WorkerAnimation.Animation = "Work";
                WorkerAnimation.Play();
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

        if (ActiveBoxes.Count == 0)
            RemoveBoxFromInventory();
    }

    private void RemoveBoxFromInventory()
    {
        var inventoryBox = InventoryBoxes.Dequeue();

        for (int i = 0; i < inventoryBox.Damage; i++)
        {
            CreateNewActiveBox(GD.RandRange(0, WordList.Length - 1), inventoryBox.damageType);
        }
        inventoryBox.QueueFree();

    }

    private void CreateNewActiveBox(int wordIndex, DamageType damageType)
    {
        var newBox = TextBoxScene.Instantiate<TextBox>();

        switch (damageType)
        {
            case DamageType.Text:
                newBox.Initialize(WordList[wordIndex], DamageType.Text);
                break;
            case DamageType.Scramble:
                string[] characterList = { "q", "w", "e", "a", "s", "d", "z", "x", "c" };
                var newWord = "";
                for (int i = 0; i < 6; i++)
                {
                    newWord += characterList[GD.RandRange(0, 8)];
                }
                newBox.Initialize(newWord, DamageType.Text);
                break;
        }


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
            intensity -= 0.3f;
        }
    }

    private void ResetActiveBox()
    {
        CharacterIndex = 0;
        HeldString = "";
    }

    private void FinishBox()
    {
        WorkerAnimation.Animation = "NewPackage";
        WorkerAnimation.Play();
        var finishedBox = ActiveBoxes.Dequeue();
        finishedBox.CompleteAnswer();
        ResetActiveBox();
        UpdateIntensity();
        if (InventoryBoxes.Count > 0 && ActiveBoxes.Count == 0)
        {
            RemoveBoxFromInventory();
        }
    }

}

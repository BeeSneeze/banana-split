using Common;
using Godot;

public partial class CustomEvents : Node
{
    [Signal]
    public delegate void PlayerTookDamageEventHandler(int HP, string DamageType);
    [Signal]
    public delegate void PlayerChangedRoomEventHandler(int roomID);
    [Signal]
    public delegate void GameOverEventHandler();
    [Signal]
    public delegate void GameWonEventHandler();
    [Signal]
    public delegate void DialogueStartedEventHandler(string npc);
    [Signal]
    public delegate void DialogueEndedEventHandler();

    public static CustomEvents Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}

using Godot;
using System.Linq;

public partial class ActionGame : Control
{
    private int CurrentRoomID = -1;
    private Room CurrentRoom;
    public Player Player { get; private set; }
    private bool RecentReparent = false;

    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://ActionGame/Characters/Player/player.tscn").Instantiate<Player>();
        Player.Position = new Vector2(0, 0);
        CurrentRoom = GetChild<Room>(0);
        Player.CurrentRoom = CurrentRoom; // NOTE: Player needs to have current room before being added to the scene!
        CurrentRoom.AddChild(Player);

        CustomEvents.Instance.PlayerChangedRoom += ChangeRoom;
        CustomEvents.Instance.GameOver += GameOver;
        CustomEvents.Instance.GameWon += GameWon;
    }

    public override void _PhysicsProcess(double delta)
    {
        // This check is needed or else the game crashes
        if (RecentReparent)
        {
            RecentReparent = false;
        }
    }

    private void GameOver()
    {
        GD.Print("GAME OVER!");
    }

    private void GameWon()
    {
        GD.Print("GAME WIN! Yay ^-^");
    }

    private void ChangeRoom(int roomID)
    {
        if (roomID == CurrentRoomID)
        {
            return; // Don't keep changing to the same room
        }

        if (CurrentRoom != null)
        {
            CurrentRoom.DeactivateRoom();
        }

        CurrentRoomID = roomID;
        CurrentRoom = (Room)GetChildren().First(x => ((Room)x).RoomID == roomID);

        GD.Print("Changed to room: " + CurrentRoomID.ToString());
        if (!RecentReparent && Player.GetParent() != CurrentRoom)
        {
            // Defer and delay to avoid crashing
            Player.GetNode<PlayerCamera>("%Camera").PositionSmoothingEnabled = false;
            Player.CallDeferred(Node2D.MethodName.Reparent, CurrentRoom);
            Player.GetNode("%Camera").CallDeferred(PlayerCamera.MethodName.TurnOnPositionSmoothing);
            RecentReparent = true;
            Player.CurrentRoom = CurrentRoom;
        }
    }

}

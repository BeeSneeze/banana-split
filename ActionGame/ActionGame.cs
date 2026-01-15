using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Schema;

public partial class ActionGame : Control
{
    private int CurrentRoomID = -1;
    private Room CurrentRoom;
    public Player Player { get; private set; }
    private const int BREAK_ROOM_INTERVAL = 7;
    private int RoomsUntilBreak = BREAK_ROOM_INTERVAL;

    private bool RoomsInitialized = false;

    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://ActionGame/Player/player.tscn").Instantiate<Player>();
        CurrentRoom = GetChild<Room>(0);
        Player.CurrentRoom = CurrentRoom; // NOTE: Player needs to have current room before being added to the scene!
        AddChild(Player);

        CustomEvents.Instance.PlayerChangedRoom += ChangeRoom;
        CustomEvents.Instance.GameOver += GameOver;
        CustomEvents.Instance.GameWon += GameWon;

        AddNewRoom();
    }

    private void AddNewRoom()
    {
        RoomsUntilBreak--;
        var roomScene = GD.Load<PackedScene>("res://ActionGame/Rooms/room_" + GD.RandRange(1, 4).ToString() + ".tscn").Instantiate<Room>();
        if (RoomsUntilBreak == 0)
        {
            RoomsUntilBreak = BREAK_ROOM_INTERVAL;
            roomScene = GD.Load<PackedScene>("res://ActionGame/Rooms/break_room.tscn").Instantiate<Room>();
        }

        roomScene.RoomID = CurrentRoomID + 1000;
        AddChild(roomScene);
        MoveRooms();
    }

    private void MoveRooms()
    {
        var childCount = 0;
        var rooms = GetChildren().Where(x => x is Room).Select(x => (Room)x).ToArray();

        for (int i = 0; i < rooms.Length - 1; i++)
        {
            childCount += rooms[i].GetNode("EntityMap").GetChildCount();
        }

        if (childCount > 0)
        {
            RoomsInitialized = true;
            for (int i = 0; i < rooms.Length - 1; i++)
            {
                foreach (var entity in rooms[i].GetNode("EntityMap").GetChildren())
                {
                    if (entity is RoomSpawnMarker marker)
                    {
                        rooms[i + 1].GlobalPosition = marker.GlobalPosition + new Vector2(-25, 25);
                    }
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!RoomsInitialized)
        {
            MoveRooms();
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

        CurrentRoom = (Room)GetChildren().Where(x => x is Room).First(x => ((Room)x).RoomID == roomID);

        GD.Print("Changed to room: " + CurrentRoomID.ToString());
        if (Player.CurrentRoom != CurrentRoom)
        {
            Player.CurrentRoom = CurrentRoom;

            var roomRectangle = CurrentRoom.GetNode<TileMapLayer>("TerrainMap").GetUsedRect();
            var cellsize = CurrentRoom.GetNode<TileMapLayer>("TerrainMap").TileSet.TileSize;

            var playerCamera = Player.GetNode<Camera2D>("%Camera");

            var zoomAmount = (float)Mathf.Clamp(30 / roomRectangle.Size.Length(), 1, 1.3);
            playerCamera.Zoom = Vector2.One * zoomAmount;


            // TODO: Figure out how to limit the camera to specific rooms
            //playerCamera.LimitLeft = roomRectangle.Position.X + (int)CurrentRoom.Position.X + 240;
            //playerCamera.LimitBottom = roomRectangle.Position.Y + (int)CurrentRoom.Position.Y;
            //playerCamera.LimitRight = roomRectangle.End.X * cellsize.X + (int)CurrentRoom.Position.X + 240;
            //playerCamera.LimitTop = roomRectangle.End.Y * cellsize. + (int)CurrentRoom.Position.Y;
        }

        AddNewRoom();
    }

}

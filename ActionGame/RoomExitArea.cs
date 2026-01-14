using Godot;
using System;

public partial class RoomExitArea : Area2D
{
    public Room Room;

    public void OnPlayerLeave()
    {
        var map = Room.GetNode<TileMapLayer>("TerrainMap");
        map.SetCellsTerrainConnect([map.LocalToMap(Position)], 0, 0);
    }
}

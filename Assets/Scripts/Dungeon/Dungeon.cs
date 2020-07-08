// ================================================================================================================================
// File:        Dungeon.cs
// Description:	Stores everything about the current floor of the dungeon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    //Dungeon configuration variables
    public int GridSize;
    public Vector2 RoomSizes;
    public int RoomCount;

    //Prefabs used
    public GameObject TilePrefab;

    //All rooms and tiles which make up this dungeon layout
    public Dictionary<Vector2, DungeonTile> Tiles = new Dictionary<Vector2, DungeonTile>();
    public List<DungeonRoom> Rooms = new List<DungeonRoom>();

    //Generates a new dungeon layout with the given settings
    public void GenerateDungeon(int GridSize, int RoomCount, Vector2 RoomSizes)
    {
        //Store generation settings
        this.GridSize = GridSize;
        this.RoomCount = RoomCount;
        this.RoomSizes = RoomSizes;

        //Generate layout
        DungeonGenerator.InitializeTileGrid(this);
        DungeonGenerator.PlaceStartingRoom(this, new Vector2(15, 15));
        DungeonGenerator.PlaceRandomRooms(this);
    }
}

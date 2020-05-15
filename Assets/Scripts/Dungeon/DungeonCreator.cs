// ================================================================================================================================
// File:        DungeonCreator.cs
// Description:	Creates a new dungeon floor, based on this article https://gamedevelopment.tutsplus.com/tutorials/create-a-procedurally-generated-dungeon-cave-system--gamedev-10099
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public GameObject DungeonTilePrefab; //Tile prefabs used when initially setting up the dungeon grid

    public int MaxRooms = 2;    //Maximum number of rooms that can exist in a single dungeon floor
    public int MinRoomSize = 1; //Minimum length of a room in tiles
    public int MaxRoomSize = 4; //Maximum length of a room in tiles
    public int DungeonWidth = 32;  //Width of tiles upon which the dungeon rooms can be placed
    public int DungeonHeight = 32; //Height of tiles upon which the dungeon rooms can be placed

    private void Start()
    {
        //Setup the initial grid of empty tiles
        Dungeon.Instance.InitializeGrid(DungeonWidth, DungeonHeight, DungeonTilePrefab);
        //Add a bunch of rooms into the dungeon
        Dungeon.Instance.PlaceRooms(MaxRooms, MinRoomSize, MaxRoomSize);
    }
}
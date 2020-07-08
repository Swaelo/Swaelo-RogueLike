// ================================================================================================================================
// File:        DungeonGenerator.cs
// Description:	Generates a random dungeon layout when the game is started
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    //Singleton instance
    public static DungeonGenerator Instance = null;
    private void Awake() { Instance = this; }

    //Layout generation parameters
    public int RoomCount = 8;   //Number of rooms that will be created to make up this floor of the dungeon
    public Vector2 RoomSizeRange = new Vector2(5, 15);  //Each rooms length/width will fall within these values
    public int GridSize = 128;  //Dimensions of the grid layout which the dungeon is created on top of

    //Prefabs
    public GameObject DungeonTilePrefab;
    public GameObject DungeonPrefab;

    //Current dungeon
    public Dungeon CurrentDungeon;

    private void Start()
    {
        GameObject NewDungeon = Instantiate(DungeonPrefab, Vector3.zero, Quaternion.identity);
        CurrentDungeon = NewDungeon.GetComponent<Dungeon>();
        CurrentDungeon.SetGenerationValues(GridSize, RoomCount, RoomSizeRange);
        CurrentDungeon.InitializeTileGrid();
        CurrentDungeon.PlaceRooms();
    }
}

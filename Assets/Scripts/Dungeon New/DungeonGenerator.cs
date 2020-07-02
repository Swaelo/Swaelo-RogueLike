// ================================================================================================================================
// File:        DungeonGenerator.cs
// Description:	Generates a random dungeon layout when the game is started
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    //Generation parameters
    public int GridSize = 128;  //Length/Width of the base grid which the dungeon is generated upon
    public int MaxRoomCount = 8;    //Maximum number of rooms that will be placed in the dungeon level
    public Vector2 RoomSizeRange = new Vector2(5, 8);   //Each rooms length/width will fall within this range

    //Tiles making up the layout of this current dungeon
    public GameObject TilePrefab;
    private Dictionary<Vector2, GameObject> DungeonTiles = new Dictionary<Vector2, GameObject>();   //All the tiles which make up the layout of this dungeon floor

    private void Start()
    {
        
    }

    //Sets up the base tile grid which will be used to build the dungeon from
    private void InitializeGrid(int Size)
    {
        //Get half dimension for offsetting all tiles so the dungeon is centered on the world origin
        float HalfSize = Size * 0.5f;

        //Initialize all the tiles and store them all in the tile dictionary
        for(int x = 0; x < Size; x++)
        {
            for(int y = 0; y < Size; y++)
            {
                //Create and initialize each new tile grid, then store it in the tile dictionary with the others
                Vector2 NewTileGridPos = new Vector2(x, y);
                Vector3 NewTileWorldPos = new Vector3(x - HalfSize, y - HalfSize, 0f);
                GameObject NewTile = Instantiate(TilePrefab, NewTileWorldPos, Quaternion.identity);
                NewTile.SendMessage("SetGridPos", NewTileGridPos);
                DungeonTiles.Add(NewTileGridPos, NewTile);
            }
        }
    }
}

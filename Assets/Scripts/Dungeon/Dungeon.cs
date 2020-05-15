// ================================================================================================================================
// File:        Dungeon.cs
// Description:	Stores all the tiles of a dungeon floor and their states
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    //Singleton Instance
    public static Dungeon Instance = null;
    private void Awake() { Instance = this; }

    //Dungeons size, room count, etc.
    public int Width;
    public int Height;

    //Lists of tiles and rooms which make up the dungeon
    public Dictionary<Vector2, DungeonTile> Tiles = new Dictionary<Vector2, DungeonTile>();   //Set of all the tiles which make up this dungeon
    public List<DungeonRoom> Rooms = new List<DungeonRoom>(); //List of all rooms inside the dungeon

    //Sets up the dungeon tiles
    public void InitializeGrid(int DungeonWidth, int DungeonHeight, GameObject TilePrefab)
    {
        //Store dungeon size values
        Width = DungeonWidth;
        Height = DungeonHeight;

        //Get half dimensions for offsetting all tiles when placing them down so the whole dungeon is centered on the world origin
        float HalfWidth = DungeonWidth * 0.5f;
        float HalfHeight = DungeonHeight * 0.5f;

        //Go through and setup all the tiles
        for(int x = 0; x < DungeonWidth; x++)
        {
            for(int y = 0; y < DungeonHeight; y++)
            {
                //Spawn in a new tile and store it in the dictionary with all the others
                Vector3 NewTilePos = new Vector3(x - HalfWidth, y - HalfHeight, 0f);
                GameObject NewTile = Instantiate(TilePrefab, NewTilePos, Quaternion.identity);
                NewTile.GetComponent<DungeonTile>().GridPos = new Vector2(x, y);
                Tiles.Add(new Vector2(x, y), NewTile.GetComponent<DungeonTile>());
            }
        }
    }

    //Randomly places a bunch of rooms into the dungeon
    public void PlaceRooms(int MaxRoomCount, int MinRoomSize, int MaxRoomSize)
    {
        //Try placing the maximum amount of rooms
        for(int i = 0; i < MaxRoomCount; i++)
        {
            //Give each room maximum number of 10 attempts to find an open available spot before giving up
            int PlacementAttempts = 1;
            bool RoomPlaced = false;
            while(PlacementAttempts < 10 && !RoomPlaced)
            {
                RoomPlaced = AddRoom(MinRoomSize, MaxRoomSize);
                PlacementAttempts++;
            }
        }
    }

    //Tries to place a random room onto the floor
    private bool AddRoom(int MinSize, int MaxSize)
    {
        //Get a random size
        int RoomWidth = Random.Range(MinSize, MaxSize + 1);
        int RoomHeight = Random.Range(MinSize, MaxSize + 1);

        //Get a random position, making sure it fits inside the current grid size
        int RoomXPos = Random.Range(RoomWidth, Width - RoomWidth - 1);
        int RoomYPos = Random.Range(RoomHeight, Height - RoomHeight - 1);

        //Setup a new room with these values
        DungeonRoom NewRoom = new DungeonRoom(RoomXPos, RoomYPos, RoomWidth, RoomHeight);

        //Check to make sure this room doesnt intersect with any other already existing rooms
        foreach(DungeonRoom OtherRoom in Rooms)
        {
            if(NewRoom.Intersects(OtherRoom))
            {
                //Exit out without finalizing the room creation if this space isnt available
                return false;
            }
        }

        //Initialize this room if the space is available
        NewRoom.Init();
        Rooms.Add(NewRoom);
        return true;
    }
}

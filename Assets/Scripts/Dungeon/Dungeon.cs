// ================================================================================================================================
// File:        Dungeon.cs
// Description:	Stores all the tiles of a dungeon floor and their states
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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

    public GameObject TilePrefab;   //Prefab used when spawning in all the tiles to setup the dungeon grid

    //Keeps track if rooms have already been placed down, so if told to generate them again we will know to clean up the previous rooms first
    private bool RoomsCreated = false;

    //Sets up the dungeon tiles
    public void InitializeGrid(int DungeonWidth, int DungeonHeight)
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

    //Cleans up and removes all of the tiles and rooms making up the current dungeon
    public void ClearDungeon()
    {
        //Remove every tile in the current grid
        foreach (KeyValuePair<Vector2, DungeonTile> Tile in Tiles)
            Destroy(Tile.Value.gameObject);
        Tiles = new Dictionary<Vector2, DungeonTile>();
        //Reset the list of rooms, and the created already flag
        Rooms = new List<DungeonRoom>();
        RoomsCreated = false;
    }

    //Randomly places a bunch of rooms into the dungeon
    public void PlaceRooms(int MaxRoomCount, int MinRoomSize, int MaxRoomSize)
    {
        //If rooms have already been placed previously, clean up the old rooms before making new ones
        if(RoomsCreated)
        {
            //Go through and return all tiles back to the empty state
            foreach (KeyValuePair<Vector2, DungeonTile> Tile in Tiles)
                Tile.Value.SetType(DungeonTile.TileType.EmptyTile);
            //Reset the rooms list now that none of the previous rooms exist anymore
            Rooms = new List<DungeonRoom>();
        }

        //Try placing the maximum amount of rooms
        for(int i = 0; i < MaxRoomCount; i++)
            TryPlaceRoom(MinRoomSize, MaxRoomSize);

        //Remember that some rooms have now been placed down
        RoomsCreated = true;
    }

    //Tries randomly placing a new room into the current dungeon grid up to a maximum of 10 times before it gives up
    private void TryPlaceRoom(int MinRoomSize, int MaxRoomSize)
    {
        int PlacementAttempts = 0;  //Tracks how many attempts have been made to place a new room into the dungeon layout
        bool PlacementComplete = false; //Tracks if a room has been succesfully placed down yet or not
        
        //Try placing down a new room 10 times before giving up
        while(!PlacementComplete && PlacementAttempts < 10)
        {
            //Track how many attempts have been made to place a new room onto the dungeon grid
            PlacementAttempts++;

            //Get a random size for the new room
            int RoomWidth = Random.Range(MinRoomSize, MaxRoomSize + 1);
            int RoomHeight = Random.Range(MinRoomSize, MaxRoomSize + 1);

            //Get a random position to place this new room inside the dungeon grid
            int RoomXPos = Random.Range(RoomWidth, Width - RoomWidth - 1);
            int RoomYPos = Random.Range(RoomHeight, Height - RoomHeight - 1);

            //Setup a new room component with these random values we just generated
            DungeonRoom NewRoom = new DungeonRoom(RoomXPos, RoomYPos, RoomWidth, RoomHeight);

            //Check to make sure the room doesnt touch or overlap any other rooms that may already exist
            bool InvalidPlacement = false;
            foreach(DungeonRoom OtherRoom in Rooms)
            {
                //Half the current placement attempt if this room placement is invalid
                if(NewRoom.RoomsOverlapping(OtherRoom))
                {
                    InvalidPlacement = true;
                    break;
                }
            }

            //Finalize the setup of this room if the placement was found to be valid
            if(!InvalidPlacement)
            {
                //Initialize the new room
                NewRoom.Init();

                //If this room isnt the first one being added to this dungeon, add corridors connecting it with the previously added room
                if(Rooms.Count != 0)
                {
                    //Get the center positions of both the new room and the previous room
                    Vector2 NewCenter = NewRoom.Center;
                    Vector2 PrevCenter = Rooms[Rooms.Count - 1].Center;

                    //Build corridors between these two locations to connect the two rooms
                    //Randomly start with either horizontal or vertical corridor first
                    if(Random.value >= 0.5f)
                    {
                        CreateHorizontalCorridor(PrevCenter.x, NewCenter.x, PrevCenter.y);
                        CreateVerticalCorridor(PrevCenter.y, NewCenter.y, NewCenter.x);
                    }
                    else
                    {
                        CreateVerticalCorridor(PrevCenter.y, NewCenter.y, PrevCenter.x);
                        CreateHorizontalCorridor(PrevCenter.x, NewCenter.x, NewCenter.y);
                    }
                }

                //Store this new room with the others, then exit out of the function
                Rooms.Add(NewRoom);
                return;
            }
        }
    }

    //Creates a horizontal corridor to connect two rooms together
    public void CreateHorizontalCorridor(float XStart, float XEnd, float Y)
    {
        //Go along the line of tiles needing to be changed into corridors
        float XMin = Mathf.Min(XStart, XEnd);
        float XMax = Mathf.Max(XStart, XEnd);
        for(int X = (int)XMin; X < XMax + 1; X++)
        {
            //Grab each tile in the line
            DungeonTile Tile = Tiles[new Vector2(X, Y)];

            //Change empty tiles into corridor tiles
            if (Tile.Type == DungeonTile.TileType.EmptyTile)
                Tile.SetType(DungeonTile.TileType.CorridorTile);
        }
    }

    //Creates a vertical corridor to connect two rooms together
    public void CreateVerticalCorridor(float YStart, float YEnd, float X)
    {
        //Go along the line of tiles needing to be changed into corridors
        float YMin = Mathf.Min(YStart, YEnd);
        float YMax = Mathf.Max(YStart, YEnd);
        for(int Y = (int)YMin; Y < YMax + 1; Y++)
        {
            //Grab each tile in the line
            DungeonTile Tile = Tiles[new Vector2(X, Y)];

            //Change empty tiles into corridor tiles
            if (Tile.Type == DungeonTile.TileType.EmptyTile)
                Tile.SetType(DungeonTile.TileType.CorridorTile);
        }
    }
}

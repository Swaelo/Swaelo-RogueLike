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
            //Get a random size for the new room
            int RoomWidth = Random.Range(MinRoomSize, MaxRoomSize + 1);
            int RoomHeight = Random.Range(MinRoomSize, MaxRoomSize + 1);

            //Get a random position for the new room, making sure it stays within the dungeon grid
            int RoomXPos = Random.Range(RoomWidth, Width - RoomWidth - 1);
            int RoomYPos = Random.Range(RoomHeight, Height - RoomHeight - 1);

            //Setup a new room with these values
            DungeonRoom NewRoom = new DungeonRoom(RoomXPos, RoomYPos, RoomWidth, RoomHeight);

            //Check to make sure this room doesnt intersect with any others
            bool Intersects = false;
            foreach(DungeonRoom OtherRoom in Rooms)
            {
                if(NewRoom.RoomsOverlapping(OtherRoom))
                {
                    //Set flag and break out if the room is found to intersect with another
                    Intersects = true;
                    break;
                }
            }

            //Initialize the new room and add it to the list with the others if its space is available
            if(!Intersects)
            {
                //Setup the new room
                NewRoom.Init();

                //Store new room center location
                Vector2 NewCenter = NewRoom.Center;

                if(Rooms.Count != 0)
                {
                    //Store center of the previous room
                    Vector2 PrevCenter = Rooms[Rooms.Count - 1].Center;

                    //Carve out corridors between these two rooms center locations
                    //Randomly start with either vertical or horizontal corridor first
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

                //Store the new room in the list with the others
                Rooms.Add(NewRoom);
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

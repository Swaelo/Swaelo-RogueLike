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
    public Vector2 StartRoomSize = new Vector2(8, 8);   //Size of the players starting room

    //Prefabs
    public GameObject DungeonTilePrefab;
    public GameObject DungeonPrefab;

    //Current dungeon
    public Dungeon CurrentDungeon;

    private void Start()
    {
        GameObject NewDungeon = Instantiate(DungeonPrefab, Vector3.zero, Quaternion.identity);
        CurrentDungeon = NewDungeon.GetComponent<Dungeon>();
        CurrentDungeon.GenerateDungeon(GridSize, RoomCount, RoomSizeRange);
    }

    //Sets up the tile grid layout for the given dungeon
    public static void InitializeTileGrid(Dungeon Dungeon)
    {
        //Get half grid size for offsetting tile placements so the layout is centered on world origin
        float HalfGridSize = Dungeon.GridSize * 0.5f;

        //Setup all the tile structs that are needed for the layout generation
        for(int x = 0; x < Dungeon.GridSize; x++)
        {
            for(int y = 0; y < Dungeon.GridSize; y++)
            {
                //Initialize each new tile structure, storing them all in the dungeons tile dictionary
                Vector2 GridPos = new Vector2(x, y);
                Vector3 WorldPos = new Vector3(x - HalfGridSize, y - HalfGridSize, 0f);
                DungeonTile NewTile = new DungeonTile(GridPos, WorldPos);
                Dungeon.Tiles.Add(GridPos, NewTile);
            }
        }
    }

    //Places the beginning room into the center of the dungeon grid, this is where the player starts at
    public static void PlaceStartingRoom(Dungeon Dungeon, Vector2 RoomSize)
    {
        //Get the position of the starting room based on its size, so its centered in the dungeon grid layout
        Vector2 RoomPos = new Vector2(Dungeon.GridSize * 0.5f, Dungeon.GridSize * 0.5f);
        RoomPos.x -= RoomSize.x / 2;
        RoomPos.y -= RoomSize.y / 2;
        //Initialize the starting room and store it with all the other rooms
        DungeonRoom StartingRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, RoomPos);
        StartingRoom.Init();
        Dungeon.Rooms.Add(StartingRoom);
    }

    //Randomly places the rest of the dungeon rooms, after the starting room has already been placed
    public static void PlaceRandomRooms(Dungeon Dungeon)
    {
        for (int i = 1; i < Dungeon.RoomCount; i++)
            PlaceRandomRoom(Dungeon);
    }

    //Randomly places a new dungeon room next to one that already exists
    private static void PlaceRandomRoom(Dungeon Dungeon)
    {
        //Get a random size for the new room
        Vector2 RoomSize = GetNewRoomSize(Dungeon.RoomSizes);

        //Create a shuffled copy of the list of rooms that already exist in the dungeon
        List<DungeonRoom> RoomsCopy = ListFunctions.Copy(Dungeon.Rooms);
        ListFunctions.Shuffle(RoomsCopy);

        //Search through the rooms, checking if we can place the new room adjacent to any of these existing rooms
        foreach(DungeonRoom OtherRoom in RoomsCopy)
        {
            //Make a list of the 4 placement directions we want to check for placing the new room
            List<Direction> Directions = new List<Direction>();
            for (int i = 0; i < 4; i++)
                Directions.Add((Direction)i);
            ListFunctions.Shuffle(Directions);

            //Use each direction to check if we can place the new room next to the current OtherRoom in that direction
            Direction Placement = Direction.North;
            bool Found = false;
            foreach(Direction Check in Directions)
            {
                //Store the direction and exit out once a valid placement location has been found
                if(CanPlaceRoom(Dungeon, RoomSize, OtherRoom, Check))
                {
                    Placement = Check;
                    Found = true;
                    break;
                }
            }

            //Add the new room to the dungeon if a valid placement location was found
            if(Found)
            {
                Vector2 NewRoomPos = OtherRoom.GetAdjacentLocation((int)RoomSize.x, (int)RoomSize.y, Placement);
                DungeonRoom NewRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, NewRoomPos);
                NewRoom.Init();
                Dungeon.Rooms.Add(NewRoom);
                ConnectRooms(Dungeon, OtherRoom, NewRoom, Placement);
                return;
                return;
            }
        }
    }

    //Returns a random size to use for the next dungeon room
    private static Vector2 GetNewRoomSize(Vector2 RoomSizes)
    {
        return new Vector2(
            Random.Range(RoomSizes.x, RoomSizes.y + 1),
            Random.Range(RoomSizes.x, RoomSizes.y + 1));
    }

    //Checks if a new room of the given size can be placed adjacent to another room in the given direction
    private static bool CanPlaceRoom(Dungeon Dungeon, Vector2 RoomSize, DungeonRoom OtherRoom, Direction PlacementDirection)
    {
        //Get the location where the new room would be placed
        Vector2 NewRoomPos = OtherRoom.GetAdjacentLocation((int)RoomSize.x, (int)RoomSize.y, PlacementDirection);

        //Make sure it will be placed at a valid location
        if (!RoomLocationValid(Dungeon, NewRoomPos, RoomSize))
            return false;

        //Make sure it wont overlap with any of the already existing rooms
        DungeonRoom NewRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, NewRoomPos);
        if (RoomOverlaps(Dungeon, NewRoom))
            return false;

        //Placement is valid if all the tests passed
        return true;
    }

    //Checks if a room of the given size can be placed at the given location
    private static bool RoomLocationValid(Dungeon Dungeon, Vector2 RoomLocation, Vector2 RoomSize)
    {
        if (RoomLocation.x < 0 ||
            RoomLocation.y < 0 ||
            RoomLocation.x + RoomSize.x >= Dungeon.GridSize ||
            RoomLocation.y + RoomSize.y >= Dungeon.GridSize)
            return false;
        return true;
    }

    //Checks if the given room will overlap with any of the already existing rooms
    private static bool RoomOverlaps(Dungeon Dungeon, DungeonRoom NewRoom)
    {
        foreach (DungeonRoom OtherRoom in Dungeon.Rooms)
            if (NewRoom.RoomsOverlapping(OtherRoom))
                return true;
        return false;
    }

    //Places doors between two adjacent rooms so the player can travel between them
    private static void ConnectRooms(Dungeon Dungeon, DungeonRoom OldRoom, DungeonRoom NewRoom, Direction PlacementDirection)
    {
        //Create two lists to contain the wall tiles of each of the two rooms that are going to be connected
        List<DungeonTile> OldRoomWallTiles = new List<DungeonTile>();
        List<DungeonTile> NewRoomWallTiles = new List<DungeonTile>();

        //Place doors between the two rooms to connect them
        switch(PlacementDirection)
        {
            case (Direction.North):
                OldRoomWallTiles = OldRoom.GetWallTiles(Direction.North);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.South);
                break;
            case (Direction.East):
                OldRoomWallTiles = OldRoom.GetWallTiles(Direction.East);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.West);
                break;
            case (Direction.South):
                OldRoomWallTiles = OldRoom.GetWallTiles(Direction.South);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.North);
                break;
            case (Direction.West):
                OldRoomWallTiles = OldRoom.GetWallTiles(Direction.West);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.East);
                break;
        }

        //Now place doors somewhere along these two walls to connect the rooms together
        bool RoomsConnected = false;
        while(!RoomsConnected)
        {
            //Grab a tile from the middle of the old rooms wall, then remove it so we dont select it again
            int OldWallSelection = (OldRoomWallTiles.Count - 1) / 2;
            DungeonTile OldWallTile = OldRoomWallTiles[OldWallSelection];
            OldRoomWallTiles.Remove(OldWallTile);

            //Grab whatever tile is adjacent to the selected tile, and check if thats part of the new rooms wall
            DungeonTile AdjacentWallTile = GetAdjacentTile(Dungeon, OldWallTile, PlacementDirection);
            bool DoesConnect = NewRoomWallTiles.Contains(AdjacentWallTile);

            //If the old rooms adjacent tile is part of the new rooms wall, this is the linking location
            if(DoesConnect)
            {
                //Connect the rooms and stop searching for a linking location
                RoomsConnected = true;
                OldWallTile.SetType(DungeonTileType.Door);
                AdjacentWallTile.SetType(DungeonTileType.Door);
            }
        }
    }

    //Returns the dungeon tile 1 space away in the given direction
    private static DungeonTile GetAdjacentTile(Dungeon Dungeon, DungeonTile Target, Direction AdjacentDirection)
    {
        Vector2 TilePos = Target.GridPos;
        switch(AdjacentDirection)
        {
            case (Direction.North):
                TilePos.y++;
                break;
            case (Direction.East):
                TilePos.x++;
                break;
            case (Direction.South):
                TilePos.y--;
                break;
            case (Direction.West):
                TilePos.x--;
                break;
        }
        return Dungeon.Tiles[TilePos];
    }
}

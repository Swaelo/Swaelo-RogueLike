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

    //New tile prefab
    public GameObject DungeonTilePrefab;

    //All tiles and rooms which make up this dungeon layout
    public Dictionary<Vector2, DungeonTile> DungeonTiles = new Dictionary<Vector2, DungeonTile>();   //Dictionary to store every tile in the dungeon
    public List<DungeonRoom> DungeonRooms = new List<DungeonRoom>();  //List of all the rooms place which make up the dungeon layout

    private void Start()
    {
        InitializeTileGrid();
        PlaceRooms();
    }

    //Sets up the tile grid layout
    private void InitializeTileGrid()
    {
        //Get half grid dimensions to offset tile placements so the dungeon is centered on the world origin
        float HalfGridSize = GridSize * 0.5f;

        for(int x = 0; x < GridSize; x++)
        {
            for(int y = 0; y < GridSize; y++)
            {
                //Initialize each new tile struct and store it in the dictionary
                Vector2 GridPos = new Vector2(x, y);
                Vector3 WorldPos = new Vector3(x - HalfGridSize, y - HalfGridSize, 0f);
                DungeonTile NewTile = new DungeonTile(GridPos, WorldPos);
                DungeonTiles.Add(GridPos, NewTile);
            }
        }
    }

    //Randomly places down a bunch of rooms to create the layout of the dungeon
    private void PlaceRooms()
    {
        //Place a first room in the center of the dungeon grid
        Vector2 RoomSize = new Vector2(8, 8);
        Vector2 RoomPos = new Vector2(GridSize * 0.5f, GridSize * 0.5f);
        RoomPos.x -= RoomSize.x / 2;
        RoomPos.y -= RoomSize.y / 2;
        DungeonRoom FirstRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, RoomPos);
        FirstRoom.Init();
        DungeonRooms.Add(FirstRoom);

        //Place the rest of the rooms connected on from the others
        for (int i = 1; i < RoomCount; i++)
            PlaceRandomRoom();
    }

    //Places a random room adjacent to one of the other already existing rooms
    private void PlaceRandomRoom()
    {
        //Get a random size for the new room
        Vector2 RoomSize = NewRoomSize();

        //Create a shuffled copy of the rooms list to use while searching for a location to place the new room
        List<DungeonRoom> RoomsCopy = ListFunctions.Copy(DungeonRooms);
        ListFunctions.Shuffle(RoomsCopy);

        //Go through the shuffled list, and check the spots adjacent to each room until a location is found for the new room
        foreach (DungeonRoom OtherRoom in RoomsCopy)
        {
            //Make a shuffled list with the 4 placement directions we want to check against this room
            List<Direction> Directions = new List<Direction>();
            for (int j = 0; j < 4; j++)
                Directions.Add((Direction)j);
            ListFunctions.Shuffle(Directions);

            //Go through these directions until a valid location is found, or we run out of sides to check
            Direction Placement = Direction.North;
            bool Found = false;
            foreach (Direction Check in Directions)
            {
                //Stop searching once a valid location is found
                if (CanPlaceRoom(RoomSize, OtherRoom, Check))
                {
                    Placement = Check;
                    Found = true;
                    break;
                }
            }

            //Setup the new room and add it into the dungeon if a valid placement location was able to be found
            if (Found)
            {
                //Place the new room at the available location that was found
                Vector2 NewRoomPos = OtherRoom.GetAdjacentLocation((int)RoomSize.x, (int)RoomSize.y, Placement);
                DungeonRoom NewRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, NewRoomPos);
                //Initialize it and store it into the list with all the others
                NewRoom.Init();
                DungeonRooms.Add(NewRoom);
                //Place doors to connect the new room with the room it was placed next to
                ConnectRooms(OtherRoom, NewRoom, Placement);
                return;
            }
        }
    }

    //Returns a random size within the given range for use in creating a new dungeon room
    private Vector2 NewRoomSize()
    {
        return new Vector2(
            Random.Range(RoomSizeRange.x, RoomSizeRange.y + 1),
            Random.Range(RoomSizeRange.x, RoomSizeRange.y + 1));
    }

    //Checks if a new room of the given size can be placed adjacent to an existing room on the given side
    private bool CanPlaceRoom(Vector2 Size, DungeonRoom Other, Direction Placement)
    {
        //Get the location where the new room would be placed
        Vector2 AdjacentPos = Other.GetAdjacentLocation((int)Size.x, (int)Size.y, Placement);

        //Make sure this new room would be contained within the current dungeon grid layout
        if (!LocationValid(AdjacentPos, Size))
            return false;

        DungeonRoom NewRoom = new DungeonRoom((int)Size.x, (int)Size.y, AdjacentPos);
        if (RoomOverlaps(NewRoom))
            return false;

        //Placement is valid if all tests have passed succesfully
        return true;
    }

    //Checks if a room placed at a given location of a given size stays inside the current grid layout
    private bool LocationValid(Vector2 Location, Vector2 Size)
    {
        if (Location.x < 0 ||
            Location.y < 0 ||
            Location.x + Size.x >= GridSize ||
            Location.y + Size.y >= GridSize)
            return false;
        return true;
    }

    //Checks to see if the new given dungeon room overlaps with any of the previously existing rooms
    private bool RoomOverlaps(DungeonRoom Room)
    {
        foreach(DungeonRoom Other in DungeonRooms)
        {
            if (Room.RoomsOverlapping(Other))
                return true;
        }
        return false;
    }

    //Connects two adjacent dungeon rooms together by placing doors between them
    private void ConnectRooms(DungeonRoom ExistingRoom, DungeonRoom NewRoom, Direction PlacementDirection)
    {
        //Create two lists to contain the wall tiles for each of the two rooms we are going to connect together
        List<DungeonTile> ExistingRoomWallTiles = new List<DungeonTile>();
        List<DungeonTile> NewRoomWallTiles = new List<DungeonTile>();

        //Place two doors in between these rooms to connect them together
        switch(PlacementDirection)
        {
            case (Direction.North):
                //Grab north wall tiles for the existing room and south wall tiles for the new room
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.North);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.South);
                break;
            case (Direction.East):
                //East wall tiles for existing, west wall tiles for new
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.East);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.West);
                break;
            case (Direction.South):
                //South wall tiles for existing, north wall tiles for new
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.South);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.North);
                break;
            case (Direction.West):
                //West wall tiles for existing, east wall tiles for new
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.West);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.East);
                break;
        }

        //Now place doors somewhere along these walls to connect the two rooms together
        AddDoors(ExistingRoom, ExistingRoomWallTiles, NewRoom, NewRoomWallTiles, PlacementDirection);
    }

    //Places doors somewhere along the two walls to link the two rooms together
    private void AddDoors(DungeonRoom FirstRoom, List<DungeonTile> FirstRoomWallTiles, DungeonRoom SecondRoom, List<DungeonTile> SecondRoomWallTiles, Direction ConnectionDirection)
    {
        //Keep grabbing a center tile from the first rooms wall, until we find one that can be connected to the adjacent room
        bool RoomsConnected = false;
        while(!RoomsConnected)
        {
            //Grab a tile from the middle of the first rooms wall
            int FirstWallSelection = (FirstRoomWallTiles.Count - 1) / 2;
            DungeonTile FirstRoomTile = FirstRoomWallTiles[FirstWallSelection];

            //Remove this tile from the list so it doesnt get chosen again if this door placement attempt fails
            FirstRoomWallTiles.Remove(FirstRoomTile);

            //Grab whatever tile is adjacent to this one in the direction of the new room and check if its part of the second rooms wall tiles
            DungeonTile AdjacentRoomTile = GetAdjacentTile(FirstRoomTile, ConnectionDirection);
            bool DoesConnect = SecondRoomWallTiles.Contains(AdjacentRoomTile);

            //IF the adjacent tile is part of the second rooms wall, then link them together
            if(DoesConnect)
            {
                //Stop searching for a place to connect the two walls together
                RoomsConnected = true;

                //Change both tiles to door tiles
                FirstRoomTile.SetType(DungeonTileType.Door);
                AdjacentRoomTile.SetType(DungeonTileType.Door);
            }
        }
    }

    //Returns the dungeon tile 1 space away in the given direction
    public DungeonTile GetAdjacentTile(DungeonTile Target, Direction AdjacentDirection)
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
        return DungeonTiles[TilePos];
    }
}

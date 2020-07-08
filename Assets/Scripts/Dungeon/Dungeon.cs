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
    private int GridSize;
    private Vector2 RoomSizes;
    private int RoomCount;

    //Prefabs used
    public GameObject TilePrefab;

    //All rooms and tiles which make up this dungeon layout
    public Dictionary<Vector2, DungeonTile> Tiles = new Dictionary<Vector2, DungeonTile>();
    public List<DungeonRoom> Rooms = new List<DungeonRoom>();

    //Sets dungeon generation configuration settings
    public void SetGenerationValues(int GridSize, int RoomCount, Vector2 RoomSizes)
    {
        this.GridSize = GridSize;
        this.RoomCount = RoomCount;
        this.RoomSizes = RoomSizes;
    }

    //Sets up the tile grid layout
    public void InitializeTileGrid()
    {
        //Get half grid size for offsetting tile placements so the dungeon is centered on the world origin
        float HalfGridSize = GridSize * 0.5f;

        //Setup all the tiles that will be used for the dungeon
        for(int x = 0; x < GridSize; x++)
        {
            for(int y = 0; y < GridSize; y++)
            {
                //Init each new tile struct, storing them all in the dictionary
                Vector2 TileGridPos = new Vector2(x, y);
                Vector3 TileWorldPos = new Vector3(x - HalfGridSize, y - HalfGridSize, 0f);
                DungeonTile NewTile = new DungeonTile(TileGridPos, TileWorldPos);
                Tiles.Add(TileGridPos, NewTile);
            }
        }
    }

    //Places a beginning room in the center of the dungeon grid where the player will start at
    public void PlaceStartingRoom(Vector2 RoomSize)
    {
        Vector2 RoomPos = new Vector2(GridSize * 0.5f, GridSize * 0.5f);
        RoomPos.x -= RoomSize.x / 2;
        RoomPos.y -= RoomSize.y / 2;
        DungeonRoom FirstRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, RoomPos);
        FirstRoom.Init();
        Rooms.Add(FirstRoom);
    }

    //Places the rest of the rooms randomly
    public void PlaceRandomRooms()
    {
        for (int i = 1; i < RoomCount; i++)
            PlaceRandomRoom();
    }

    //Places a room randomly next to one of the already existing rooms
    private void PlaceRandomRoom()
    {
        //Get a random size for the new room
        Vector2 RoomSize = GetRandomRoomSize();

        //Create a shuffled copy of the current rooms list to use while searching for a spot to place this new room
        List<DungeonRoom> RoomsCopy = ListFunctions.Copy(Rooms);
        ListFunctions.Shuffle(RoomsCopy);

        //Go through the shuffled list, and check each room until we find a spot to place the new room next to one of them
        foreach(DungeonRoom OtherRoom in RoomsCopy)
        {
            //Make a shuffled list with the 4 placement directions we want to check for placing the new room
            List<Direction> Directions = new List<Direction>();
            for (int i = 0; i < 4; i++)
                Directions.Add((Direction)i);
            ListFunctions.Shuffle(Directions);

            //Go through and use these directions to check each side of the other room for a valid placement location
            Direction Placement = Direction.North;
            bool Found = false;
            foreach(Direction Check in Directions)
            {
                //Store the valid placement location and break out of the loops once its found
                if(CanPlaceRoom(RoomSize, OtherRoom, Check))
                {
                    Placement = Check;
                    Found = true;
                    break;
                }
            }

            //Setup the new room and add it to the dungeon if a valid placement location was able to be found
            if (Found)
            {
                //Place the new room at the location that was found
                Vector2 NewRoomPos = OtherRoom.GetAdjacentLocation((int)RoomSize.x, (int)RoomSize.y, Placement);
                DungeonRoom NewRoom = new DungeonRoom((int)RoomSize.x, (int)RoomSize.y, NewRoomPos);
                NewRoom.Init();
                Rooms.Add(NewRoom);
                ConnectRooms(OtherRoom, NewRoom, Placement);
                return;
            }
        }
    }

    //Returns a random size to use for the next new dungeon room
    private Vector2 GetRandomRoomSize()
    {
        return new Vector2(
            Random.Range(RoomSizes.x, RoomSizes.y + 1),
            Random.Range(RoomSizes.x, RoomSizes.y + 1));
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
        foreach (DungeonRoom Other in Rooms)
        {
            if (Room.RoomsOverlapping(Other))
                return true;
        }
        return false;
    }

    private void ConnectRooms(DungeonRoom ExistingRoom, DungeonRoom NewRoom, Direction PlacementDirection)
    {
        //Create two lists to contain the wall tiles for each of the two rooms we are going to connect together
        List<DungeonTile> ExistingRoomWallTiles = new List<DungeonTile>();
        List<DungeonTile> NewRoomWallTiles = new List<DungeonTile>();

        //Place two doors in between these rooms to connect them together
        switch (PlacementDirection)
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
        while (!RoomsConnected)
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
            if (DoesConnect)
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
        switch (AdjacentDirection)
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
        return Tiles[TilePos];
    }
}

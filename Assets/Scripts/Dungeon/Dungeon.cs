// ================================================================================================================================
// File:        Dungeon.cs
// Description:	Stores all the tiles of a dungeon floor and their states
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Dungeon : MonoBehaviour
{
    //Singleton Instance
    public static Dungeon Instance = null;
    private void Awake() { Instance = this; }

    //Dungeons size, room count, etc.
    public int Width;
    public int Height;
    public int MaxRooms;
    public int RoomMinSize;
    public int RoomMaxSize;

    //Lists of tiles and rooms which make up the dungeon
    public Dictionary<Vector2, DungeonTile> Tiles = new Dictionary<Vector2, DungeonTile>();   //Set of all the tiles which make up this dungeon
    public List<DungeonRoom> Rooms = new List<DungeonRoom>(); //List of all rooms inside the dungeon

    public GameObject TilePrefab;   //Prefab used when spawning in all the tiles to setup the dungeon grid

    //Keeps track if rooms have already been placed down, so if told to generate them again we will know to clean up the previous rooms first
    private bool RoomsCreated = false;

    private void Start()
    {
        InitializeGrid(Width, Height);
        PlaceRoomsTouching(MaxRooms, RoomMinSize, RoomMaxSize);
        Player.Instance.SetHidden(false);
    }

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

    //Cleans up any existing dungeon rooms
    private void CleanupRooms()
    {
        //Go through and return all tiles back to the empty state
        foreach (KeyValuePair<Vector2, DungeonTile> Tile in Tiles)
            Tile.Value.SetType(DungeonTile.TileType.EmptyTile);
        //Reset the rooms list now that none of the previous rooms exist anymore
        Rooms = new List<DungeonRoom>();
    }

    //Randomly places a bunch of rooms into the dungeon, making sure they're all spaced apart by atleast 1 space
    public void PlaceRoomsSpaced(int MaxRoomCount, int MinRoomSize, int MaxRoomSize)
    {
        //If rooms have already been placed previously, clean up the old rooms before making new ones
        if (RoomsCreated)
            CleanupRooms();

        //Try placing the maximum amount of rooms
        for(int i = 0; i < MaxRoomCount; i++)
            TryPlaceSpacedRoom(MinRoomSize, MaxRoomSize);

        //Remember that some rooms have now been placed down
        RoomsCreated = true;
    }

    //Randomly places a bunch of rooms into the dungeon, making sure they're all touching each other
    public void PlaceRoomsTouching(int MaxRoomCount, int MinRoomSize, int MaxRoomSize)
    {
        //Clean up previous rooms if any already exist
        if (RoomsCreated)
            CleanupRooms();

        //Try placing the maximum amount of rooms
        PlaceFirstRoom(MinRoomSize, MaxRoomSize);
        for (int i = 1; i < MaxRoomCount; i++)
            TryPlaceCompactedRoom(MinRoomSize, MaxRoomSize);

        //Remember that some rooms now have been placed down
        RoomsCreated = true;
    }

    //Tries randomly placing a new room into the current dungeon grid up to a maximum of 10 times before it gives up, makes sure this rooms is spaced apart from all others by atleast 1 space
    private void TryPlaceSpacedRoom(int MinRoomSize, int MaxRoomSize)
    {
        int PlacementAttempts = 0;  //Tracks how many attempts have been made to place a new room into the dungeon layout
        bool PlacementComplete = false; //Tracks if a room has been succesfully placed down yet or not
        
        //Try placing down a new room 10 times before giving up
        while(!PlacementComplete && PlacementAttempts < 10)
        {
            //Track how many attempts have been made to place a new room onto the dungeon grid
            PlacementAttempts++;

            //Get a random size and position for the new room
            Vector2 RoomSize = GetRandomRoomSize(MinRoomSize, MaxRoomSize);
            Vector2 RoomPos = GetRandomRoomPos(RoomSize);

            //Setup a new room component with these random values we just generated
            DungeonRoom NewRoom = new DungeonRoom(RoomPos, RoomSize);

            //Check to make sure the room doesnt touch or overlap any other rooms that may already exist
            bool InvalidPlacement = false;
            foreach(DungeonRoom OtherRoom in Rooms)
            {
                //Half the current placement attempt if this room placement is invalid
                if(NewRoom.RoomsOverlapping(OtherRoom) && !NewRoom.RoomsAdjacent(OtherRoom))
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

    //Places down the first room in the center of the dungeon grid
    private void PlaceFirstRoom(int MinSize, int MaxSize)
    {
        Vector2 RoomSize = GetRandomRoomSize(MinSize, MaxSize);
        Vector2 RoomPos = new Vector2(Width * 0.5f, Height * 0.5f);
        RoomPos.x -= RoomSize.x / 2;
        RoomPos.y -= RoomSize.y / 2;
        DungeonRoom NewRoom = new DungeonRoom(RoomPos, RoomSize);
        NewRoom.Init();
        Rooms.Add(NewRoom);
    }

    //Checks if a new room of the given size can be placed adjacent to the given already existing room in the given direction
    private bool CanPlaceAdjacent(Vector2 NewRoomSize, DungeonRoom OtherRoom, Direction PlacementDirection)
    {
        //Get the location where the new room would be placed
        Vector2 AdjacentPos = OtherRoom.GetAdjacentLocation(NewRoomSize, PlacementDirection);

        //Make sure the new room will be contained inside the current dungeon grid
        if (!RoomLocationValid(AdjacentPos, NewRoomSize))
            return false;

        //Make sure a room placed at this new location doesnt overlap with any of the other already existing rooms
        DungeonRoom NewRoom = new DungeonRoom(AdjacentPos, NewRoomSize);
        if (RoomOverlaps(NewRoom))
            return false;

        //Return true if the new room doesnt overlap with any others
        return true;
    }

    //Checks if a room can be placed at a given location with the given size and still remain inside the current dungeon grid
    private bool RoomLocationValid(Vector2 RoomLocation, Vector2 RoomSize)
    {
        if (RoomLocation.x < 0 ||
            RoomLocation.y < 0 ||
            RoomLocation.x + RoomSize.x >= Width ||
            RoomLocation.y + RoomSize.y >= Height)
            return false;
        return true;
    }

    //Tries randomly placing a new room into the current dungeon grid up to a maximum of 10 times before it gives up, makes sure this rooms is placed right next to one of the others
    private void TryPlaceCompactedRoom(int MinRoomSize, int MaxRoomSize)
    {
        //Get a random size for the new room thats going to be placed down
        Vector2 RoomSize = GetRandomRoomSize(MinRoomSize, MaxRoomSize);

        //Create a shuffled copy of the rooms list to use when searching for an adjacent location to place the new room
        List<DungeonRoom> RoomsCopy = ListFunctions.Copy(Rooms);
        ListFunctions.Shuffle(RoomsCopy);

        //Go through each room which already exists, and try placing a new room adjacent to it on one of its 4 sides
        //If a new room cant be placed on any of its 4 sides, go on to the next room in the list
        foreach(DungeonRoom OtherRoom in RoomsCopy)
        {
            //Create a list with the 4 directions we want to check on, so they can be taken from it at random
            List<Direction> CheckDirections = new List<Direction>();
            for (int i = 0; i < 4; i++)
                CheckDirections.Add((Direction)i);
            ListFunctions.Shuffle(CheckDirections);

            //Store valid placement direction here once its found
            Direction PlacementDirection = Direction.North;
            bool DirectionFound = false;

            //Check each direction in the list until we find a valid location to place the new room, or run out of sides to check for
            foreach(Direction CheckDirection in CheckDirections)
            {
                //Store the placement direction and stop searching once a valid placement location has been found
                if (CanPlaceAdjacent(RoomSize, OtherRoom, CheckDirection))
                {
                    PlacementDirection = CheckDirection;
                    DirectionFound = true;
                    break;
                }
            }

            //Setup the new room and add it into the dungeon if we were able to find a valid location to place it at
            if(DirectionFound)
            {
                //Place a new room at the available location that was found
                Vector2 NewRoomPos = OtherRoom.GetAdjacentLocation(RoomSize, PlacementDirection);
                DungeonRoom NewRoom = new DungeonRoom(NewRoomPos, RoomSize);
                //Initialize the new room and store it in the list with all the others
                NewRoom.Init();
                Rooms.Add(NewRoom);
                //Place doors to connect the new room with the room that it was placed next to
                ConnectRooms(OtherRoom, NewRoom, PlacementDirection);
                return;
            }
        }
    }

    //Checks if the given room overlaps with any of the already existing rooms
    private bool RoomOverlaps(DungeonRoom NewRoom)
    {
        foreach(DungeonRoom OtherRoom in Rooms)
        {
            if (NewRoom.RoomsOverlapping(OtherRoom))
                return true;
        }
        return false;
    }

    //Returns a random size within the given range to use as the size for a new room
    private Vector2 GetRandomRoomSize(int MinSize, int MaxSize)
    {
        return new Vector2(
            Random.Range(MinSize, MaxSize + 1),
            Random.Range(MinSize, MaxSize + 1));
    }

    //Returns a random position to place a new room in the dungeon
    private Vector2 GetRandomRoomPos(Vector2 RoomSize)
    {
        return new Vector2(
            Random.Range(RoomSize.x, Width - RoomSize.x - 1),
            Random.Range(RoomSize.y, Height - RoomSize.y - 1));
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

    //Connects two adjacent dungeon rooms by placing doors to travel between them
    private void ConnectRooms(DungeonRoom ExistingRoom, DungeonRoom NewRoom, Direction PlacementDirection)
    {
        //Create two lists to contain the wall tiles for each of the two rooms we are going to connect together
        List<DungeonTile> ExistingRoomWallTiles = new List<DungeonTile>();
        List<DungeonTile> NewRoomWallTiles = new List<DungeonTile>();

        //Place two doors in between the two rooms to connect them together
        switch(PlacementDirection)
        {
            case (Direction.North):
                //Grab the north wall tiles of the existing room and the south wall tiles of the new room
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.North);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.South);
                break;
            case (Direction.East):
                //Grab the east wall of the existing room, and west of the new room
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.East);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.West);
                break;
            case (Direction.South):
                //South for existing, north for new
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.South);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.North);
                break;
            case (Direction.West):
                //West for existing, east for new
                ExistingRoomWallTiles = ExistingRoom.GetWallTiles(Direction.West);
                NewRoomWallTiles = NewRoom.GetWallTiles(Direction.East);
                break;
        }

        //Now place doors somewhere along these walls to connect the two rooms together
        AddDoors(ExistingRoom, ExistingRoomWallTiles, NewRoom, NewRoomWallTiles, PlacementDirection);
    }

    //Places doors along the two rooms adjacent walls to link them together
    private void AddDoors(DungeonRoom FirstRoom, List<DungeonTile> FirstRoomWallTiles, DungeonRoom SecondRoom, List<DungeonTile> SecondRoomWallTiles, Direction ConnectionDirection)
    {

        //Keep grabbing a tile from the first rooms wall, until we find one that can be connected to the adjacent room
        bool RoomsConnected = false;
        while(!RoomsConnected)
        {
            //Grab a tile from the first rooms wall which is most towards the center of that room
            int FirstWallSelection = (FirstRoomWallTiles.Count - 1) / 2;
            DungeonTile FirstRoomTile = FirstRoomWallTiles[FirstWallSelection];

            //Remove this tile from the list so if this placement attempt fails, we dont try using the same tile again
            FirstRoomWallTiles.Remove(FirstRoomTile);

            //Grab whatever tile is adjacent to this one in the direction of the new room, check if that adjacent tile is one of the second rooms wall tiles
            DungeonTile AdjacentRoomTile = GetAdjacentTile(FirstRoomTile, ConnectionDirection);
            bool DoesConnect = SecondRoomWallTiles.Contains(AdjacentRoomTile);

            //If the adjacent tile is part of the second rooms wall, then link them all together
            if (DoesConnect)
            {
                //Stop searching for a place to connect these two rooms
                RoomsConnected = true;

                //Change both tiles to door tiles
                FirstRoomTile.SetType(DungeonTile.TileType.RoomDoor);
                AdjacentRoomTile.SetType(DungeonTile.TileType.RoomDoor);

                //Set the destination room for each door to the adjacent room
                FirstRoomTile.DestinationRoom = SecondRoom;
                AdjacentRoomTile.DestinationRoom = FirstRoom;

                //Find and set the destination tiles for each door
                FirstRoomTile.DestinationTile = GetAdjacentTile(AdjacentRoomTile, ConnectionDirection);
                AdjacentRoomTile.DestinationTile = GetReverseAdjacentTile(FirstRoomTile, ConnectionDirection);
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
        return Tiles[TilePos];
    }

    //Returns the dungeon tile 1 space away in the opposite of the given direction
    public DungeonTile GetReverseAdjacentTile(DungeonTile Target, Direction AdjacentDirection)
    {
        Vector2 TilePos = Target.GridPos;
        switch (AdjacentDirection)
        {
            case (Direction.North):
                TilePos.y--;
                break;
            case (Direction.East):
                TilePos.x--;
                break;
            case (Direction.South):
                TilePos.y++;
                break;
            case (Direction.West):
                TilePos.x++;
                break;
        }
        return Tiles[TilePos];
    }
}

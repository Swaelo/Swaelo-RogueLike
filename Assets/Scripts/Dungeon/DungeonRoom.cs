// ================================================================================================================================
// File:        DungeonRoom.cs
// Description:	Defines a room in the dungeon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{
    public Vector2 RoomSize;    //Width and Height of the room
    public Vector2 RoomPos; //Rooms grid coordinates
    public Vector2 Center;  //Center point of the room

    private Vector2 XRange; //X Axis grid coordinates for furthest Left/Right cells of the room
    private Vector2 YRange; //Y Axis grid coordinates for furthest Bottom/Top cells of the room

    public Dictionary<Vector2, DungeonTile> Tiles = new Dictionary<Vector2, DungeonTile>(); //All the tiles used to make up this dungeon room

    //Returns a list of the tiles which make up one of the walls of this room (does not include the corner tiles)
    public List<DungeonTile> GetWallTiles(Direction RoomSide)
    {
        //Make a new list to store the wall tiles
        List<DungeonTile> WallTiles = new List<DungeonTile>();

        //Add the tiles to the list based on which side was requested
        switch(RoomSide)
        {
            case (Direction.North):
                for (int i = 2; i < RoomSize.x; i++)
                    WallTiles.Add(Tiles[new Vector2(i, RoomSize.y)]);
                break;
            case (Direction.East):
                for (int i = 2; i < RoomSize.y; i++)
                    WallTiles.Add(Tiles[new Vector2(RoomSize.x, i)]);
                break;
            case (Direction.South):
                for (int i = 2; i < RoomSize.x; i++)
                    WallTiles.Add(Tiles[new Vector2(i, 1)]);
                break;
            case (Direction.West):
                for (int i = 2; i < RoomSize.y; i++)
                    WallTiles.Add(Tiles[new Vector2(1, i)]);
                break;
        }
        
        //Return the final list of wall tiles
        return WallTiles;
    }

    //Constructor
    public DungeonRoom(Vector2 Position, Vector2 Size)
    {
        //Store size and pos values
        RoomPos = Position;
        RoomSize = Size;

        //Get and store the rooms X/Y ranges
        XRange = new Vector2(RoomPos.x, RoomPos.x + RoomSize.x);
        YRange = new Vector2(RoomPos.y, RoomPos.y + RoomSize.y);

        //Find and store center location
        Center = new Vector2(
            Mathf.Floor((RoomPos.x + (RoomPos.x + RoomSize.x)) / 2),
            Mathf.Floor((RoomPos.y + (RoomPos.y + RoomSize.y)) / 2));

    }

    //Sets up the room to be displayed
    public void Init()
    {
        //Track the row and column coordinates of each tile in relation to its position in this room of the dungeon
        int Column = 1;
        int Row = 1;

        //Change all the tiles which make up this room into room types
        for (int x = (int)XRange.x; x < (int)XRange.y; x++)
        {
            for (int y = (int)YRange.x; y < (int)YRange.y; y++)
            {
                //Grab the tile being initialised
                DungeonTile Tile = Dungeon.Instance.Tiles[new Vector2(x, y)];

                //Tell the tile its coordinates inside this room, and which room it belongs to
                Vector2 TilePos = new Vector2(Column, Row);
                Tile.RoomPos = TilePos;
                Tile.Room = this;

                //Store this tile in the dictionary of tiles used to make up this room
                Tiles.Add(new Vector2(Column, Row), Tile);

                //Check which side of the room this tile lies on
                bool IsLeft = x == (int)XRange.x;
                bool IsRight = x == ((int)XRange.y) - 1;
                bool IsBottom = y == (int)YRange.x;
                bool IsTop = y == ((int)YRange.y) - 1;

                //Set the tiles type based on which sides of the room its on
                //Top-Left Corner
                if (IsTop && IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomTopLeftTile);
                //Top Wall
                else if (IsTop && !IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomTopTile);
                //Top-Right Corner
                else if (IsTop && !IsLeft && IsRight && !IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomTopRightTile);
                //Right Wall
                else if (!IsTop && !IsLeft && IsRight && !IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomRightTile);
                //Bottom-Right Corner
                else if (!IsTop && !IsLeft && IsRight && IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomBottomRightTile);
                //Bottom Wall
                else if (!IsTop && !IsLeft && !IsRight && IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomBottomTile);
                //Bottom-Left Corner
                else if (!IsTop && IsLeft && !IsRight && IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomBottomLeftTile);
                //Left Wall
                else if (!IsTop && IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTile.TileType.RoomLeftTile);
                //Middle Tile
                else
                    Tile.SetType(DungeonTile.TileType.RoomMiddleTile);

                Row++;
            }
            Column++;
            Row = 1;
        }
    }

    //Checks to see if this room is overlapping with the other
    public bool RoomsOverlapping(DungeonRoom Other)
    {
        return XRange.x < Other.XRange.x + Other.RoomSize.y &&
            XRange.x + RoomSize.x > Other.XRange.x &&
            YRange.x < Other.YRange.x + Other.RoomSize.y &&
            YRange.x + RoomSize.y > Other.YRange.x;
    }

    //Checks if this room is sitting adjacent to the other
    public bool RoomsAdjacent(DungeonRoom Other)
    {
        //Return false if the rooms are overlapping one another
        if (RoomsOverlapping(Other))
            return false;

        //Check if the rooms are adjacent to each other on the X axis
        bool AdjacentLeft = XRange.x - 1 == Other.XRange.y;
        bool AdjacentRight = XRange.y + 1 == Other.XRange.y;
        //Check if the rooms are adjacent to each other on the Y axis
        bool AdjacentDown = YRange.x - 1 == Other.YRange.x;
        bool AdjacentUp = YRange.y + 1 == Other.YRange.y;

        //Return true if the rooms are adjacent on any side
        return AdjacentLeft || AdjacentRight || AdjacentDown || AdjacentUp;
    }

    //Returns the grid coordinates of where a room should be placed to be adjacent to this on any specific side
    public Vector2 GetAdjacentLocation(Vector2 AdjacentRoomSize, Direction RoomDirection)
    {
        //Start with the location of the current room
        Vector2 AdjacentRoomPos = RoomPos;

        //Offset from this location in the given direction
        switch (RoomDirection)
        {
            case (Direction.North):
                AdjacentRoomPos.y += RoomSize.y;
                break;
            case (Direction.East):
                AdjacentRoomPos.x += RoomSize.x;
                break;
            case (Direction.South):
                AdjacentRoomPos.y -= AdjacentRoomSize.y;
                break;
            case (Direction.West):
                AdjacentRoomPos.x -= AdjacentRoomSize.x;
                break;
        }

        //Return the final adjacent room location
        return AdjacentRoomPos;
    }
}

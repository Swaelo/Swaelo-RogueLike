// ================================================================================================================================
// File:        DungeonRoom.cs
// Description:	Defines a room in the dungeon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonRoom
{
    public Vector2 RoomSize;    //Width and Height of the room
    public Vector2 RoomPos; //Rooms grid coordinates
    public Vector2 Center;  //Center point of the room

    private Vector2 XRange; //X Axis grid coordinates for furthest Left/Right cells of the room
    private Vector2 YRange; //Y Axis grid coordinates for furthest Bottom/Top cells of the room

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
        //Change all the tiles which make up this room into room types
        for (int x = (int)XRange.x; x < (int)XRange.y; x++)
        {
            for (int y = (int)YRange.x; y < (int)YRange.y; y++)
            {
                //Grab the tile being initialised
                DungeonTile Tile = Dungeon.Instance.Tiles[new Vector2(x, y)];

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
            }
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

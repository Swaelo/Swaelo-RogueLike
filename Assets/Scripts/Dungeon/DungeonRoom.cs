// ================================================================================================================================
// File:        DungeonRoom.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoom
{
    public int RoomWidth;
    public int RoomHeight;
    public Vector2 RoomPos; //The grid position of the top left tile in this room
    public Vector2 Center;  //Center point of the room
    public Dictionary<Vector2, DungeonTile> RoomTiles = new Dictionary<Vector2, DungeonTile>();
    private Vector2 XRange; //X Axis grid coordinates for furthest left and right tiles of the room
    private Vector2 YRange; //Y Axis grid coordinates for furthest left and right tiles of the room

    public DungeonRoom(int Width, int Height, Vector2 RoomPos)
    {
        //Store size and position values
        this.RoomPos = RoomPos;
        RoomWidth = Width;
        RoomHeight = Height;

        //Find and store X/Y range coordinates
        XRange = new Vector2(RoomPos.x, RoomPos.x + RoomWidth);
        YRange = new Vector2(RoomPos.y, RoomPos.y + RoomHeight);

        //Find the center location of this room
        Center = new Vector2(
            Mathf.Floor((RoomPos.x + (RoomPos.x + RoomWidth)) / 2),
            Mathf.Floor((RoomPos.y + (RoomPos.y + RoomHeight)) / 2));
    }

    public void Init()
    {
        //Track the row and column coordinates of each tile in relation to its position in this room of the dungeon
        int Column = 1;
        int Row = 1;

        //Change all the tiles which make up this room into their set room types
        for (int x = (int)XRange.x; x < (int)XRange.y; x++)
        {
            for(int y = (int)YRange.x; y < (int)YRange.y; y++)
            {
                //Grab the tile being initialised, store it in this rooms tile dictionary
                DungeonTile Tile = DungeonGenerator.Instance.CurrentDungeon.Tiles[new Vector2(x, y)];
                RoomTiles.Add(new Vector2(Column, Row), Tile);

                bool IsLeft = x == (int)XRange.x;
                bool IsRight = x == ((int)XRange.y) - 1;
                bool IsBottom = y == (int)YRange.x;
                bool IsTop = y == ((int)YRange.y) - 1;

                //Set the tiles type based on which sides of the room its on
                if (IsTop && IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTileType.TopLeftWall);
                else if (IsTop && !IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTileType.TopWall);
                else if (IsTop && !IsLeft && IsRight && !IsBottom)
                    Tile.SetType(DungeonTileType.TopRightWall);
                else if (!IsTop && !IsLeft && IsRight && !IsBottom)
                    Tile.SetType(DungeonTileType.RightWall);
                else if (!IsTop && !IsLeft && IsRight && IsBottom)
                    Tile.SetType(DungeonTileType.BottomRightWall);
                else if (!IsTop && !IsLeft && !IsRight && IsBottom)
                    Tile.SetType(DungeonTileType.BottomWall);
                else if (!IsTop && IsLeft && !IsRight && IsBottom)
                    Tile.SetType(DungeonTileType.BottomLeftWall);
                else if (!IsTop && IsLeft && !IsRight && !IsBottom)
                    Tile.SetType(DungeonTileType.LeftWall);
                else
                    Tile.SetType(DungeonTileType.Floor);

                Row++;
            }
            Column++;
            Row = 1;
        }
    }

    //Returns the coordinates of where a new room should be placed to be adjacent to this in the given direction
    public Vector2 GetAdjacentLocation(int Width, int Height, Direction Placement)
    {
        //Start with the location of the current room
        Vector2 AdjacentPos = RoomPos;

        //Offset from this location in the given placement direction
        switch(Placement)
        {
            case (Direction.North):
                AdjacentPos.y += RoomHeight;
                break;
            case (Direction.East):
                AdjacentPos.x += RoomWidth;
                break;
            case (Direction.South):
                AdjacentPos.y -= Height;
                break;
            case (Direction.West):
                AdjacentPos.x -= Width;
                break;
        }

        //Return the adjacent rooms placement location
        return AdjacentPos;
    }

    //Checks to see if this room overlaps with the other given room
    public bool RoomsOverlapping(DungeonRoom Other)
    {
        return XRange.x < Other.XRange.x + Other.RoomHeight &&
            XRange.x + RoomWidth > Other.XRange.x &&
            YRange.x < Other.YRange.x + Other.RoomHeight &&
            YRange.x + RoomHeight > Other.YRange.x;
    }

    //Returns a list of the tiles which make up one of the walls of this room (does not include the wall corners)
    public List<DungeonTile> GetWallTiles(Direction RoomSide)
    {
        //Make a new list to store the wall tiles
        List<DungeonTile> WallTiles = new List<DungeonTile>();

        //Add the tiles to the list based on which side was given
        switch (RoomSide)
        {
            case (Direction.North):
                for (int i = 2; i < RoomWidth; i++)
                {
                    Vector2 TilePos = new Vector2(i, RoomHeight);
                    if (RoomTiles.ContainsKey(TilePos))
                        WallTiles.Add(RoomTiles[TilePos]);
                }
                break;
            case (Direction.East):
                for (int i = 2; i < RoomHeight; i++)
                {
                    Vector2 TilePos = new Vector2(RoomWidth, i);
                    if (RoomTiles.ContainsKey(TilePos))
                        WallTiles.Add(RoomTiles[TilePos]);
                }
                break;
            case (Direction.South):
                for (int i = 2; i < RoomWidth; i++)
                {
                    Vector2 TilePos = new Vector2(i, 1);
                    if (RoomTiles.ContainsKey(TilePos))
                        WallTiles.Add(RoomTiles[TilePos]);
                }
                break;
            case (Direction.West):
                for (int i = 2; i < RoomHeight; i++)
                {
                    Vector2 TilePos = new Vector2(1, i);
                    if (RoomTiles.ContainsKey(TilePos))
                        WallTiles.Add(RoomTiles[TilePos]);
                }
                break;
        }

        //Return the final list of wall tiles
        return WallTiles;
    }
}
// ================================================================================================================================
// File:        DungeonRoom.cs
// Description:	Defines a room in the dungeon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonRoom
{
    public int MinX;    //XPos Left
    public int MaxX;    //XPos Right
    public int MinY;    //YPos Top
    public int MaxY;    //YPos Bottom

    public int Width;   //Width of the room in tiles
    public int Height;  //Height of the room in tiles

    public Vector2 Center;  //Center point of the room

    //Initializes a new DungeonRoom object with the given values
    public DungeonRoom(int X, int Y, int W, int H)
    {
        MinX = X;
        MaxX = X + W;
        MinY = Y;
        MaxY = Y + H;
        Width = W;
        Height = H;
        Center = new Vector2(Mathf.Floor((MinX + MaxX) / 2),
            Mathf.Floor((MinY + MaxY) / 2));
    }

    //Sets up the room to be displayed
    public void Init()
    {
        //Change all the tiles which make up this room into room types
        for (int x = MinX; x < MaxX; x++)
        {
            for (int y = MinY; y < MaxY; y++)
            {
                //Grab the tile being initialised
                DungeonTile Tile = Dungeon.Instance.Tiles[new Vector2(x, y)];

                //Check which side of the room this tile lies on
                bool IsLeft = x == MinX;
                bool IsRight = x == MaxX - 1;
                bool IsBottom = y == MinY;
                bool IsTop = y == MaxY - 1;

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

    //Checks to see if this room is either interesting with the other, or sitting directly next to it
    public bool RoomsOverlapping(DungeonRoom Other)
    {
        return MinX < Other.MinX + Other.Width + 1 &&
            MinX + Width + 1 > Other.MinX &&
            MinY < Other.MinY + Other.Height + 1 &&
            MinY + Height + 1 > Other.MinY;
    }
}

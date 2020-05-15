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

    //Initializes a new DungeonRoom object with the given values
    public DungeonRoom(int X, int Y, int W, int H)
    {
        MinX = X;
        MaxX = X + W;
        MinY = Y;
        MaxY = Y + H;
        Width = W;
        Height = H;
    }

    //Sets up the room to be displayed
    public void Init()
    {
        //Change all the tiles which make up this room into room types
        for (int x = MinX; x < MaxX; x++)
        {
            for (int y = MinY; y < MaxY; y++)
            {
                Dungeon.Instance.Tiles[new Vector2(x, y)].SetType(DungeonTile.TileType.RoomTile);
            }
        }
    }

    //Checks if this room intersects with another
    public bool Intersects(DungeonRoom Other)
    {
        return MinX < Other.MinX + Other.Width &&
            MinX + Width > Other.MinX &&
            MinY < Other.MinY + Other.Height &&
            MinY + Height > Other.MinY;
    }
}

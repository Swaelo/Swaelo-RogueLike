// ================================================================================================================================
// File:        DungeonTile.cs
// Description:	Defines a single tile in the dungeon, can change it state
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    //Each type of tile type that is available
    public enum TileType
    {
        EmptyTile = 0,
        RoomMiddleTile = 1,
        RoomTopTile = 2,
        RoomTopRightTile = 3,
        RoomRightTile = 4,
        RoomBottomRightTile = 5,
        RoomBottomTile = 6,
        RoomBottomLeftTile = 7,
        RoomLeftTile = 8,
        RoomTopLeftTile = 9,
        CorridorTile = 10
    }

    //Stores the sprites for each tile type
    [System.Serializable]
    public struct TileSprite
    {
        public TileType Type;
        public Sprite Sprite;
    }
    public TileSprite[] TileSprites;

    public Vector2 GridPos; //Tiles coordinates in the dungeon grid

    //Changes the tile between types and updates its sprite
    public TileType Type = TileType.EmptyTile;   //This tiles current type
    public SpriteRenderer Renderer; //This tiles sprite renderer component
    public void SetType(TileType NewType)
    {
        //Store the new sprite type
        Type = NewType;

        //Look through the sprites dictionary until we find the new type that is being set
        foreach(TileSprite TSprite in TileSprites)
        {
            //Set the new type and exit the dungeon once its found
            if(TSprite.Type == NewType)
            {
                Renderer.sprite = TSprite.Sprite;
                return;
            }
        }
    }
}

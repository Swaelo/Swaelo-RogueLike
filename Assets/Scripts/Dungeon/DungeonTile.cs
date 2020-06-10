// ================================================================================================================================
// File:        DungeonTile.cs
// Description:	Defines a single tile in the dungeon, can change it state
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

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
        CorridorTile = 10,
        RoomDoor = 11
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
    public Vector2 RoomPos; //Tiles coordinates in the dungeon room it belongs to
    public DungeonRoom Room;    //The room this tile belongs to

    public DungeonRoom DestinationRoom; //If this tile is a door, this is the room that the player will travel to if they go through the door
    public DungeonTile DestinationTile; //If this tile is a door, this is the tile that the player will travel to if they go through the door

    public BoxCollider2D TileCollider;
    public void ToggleCollider(bool Enable)
    {
        TileCollider.enabled = Enable;
    }
    private void Awake() { ToggleCollider(false); }

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
                ToggleCollider(NewType);
                break;
            }
        }

        //Update the tiles tag
        if (NewType == TileType.RoomDoor)
            gameObject.tag = "Door";
        else if (NewType == TileType.RoomBottomLeftTile ||
            NewType == TileType.RoomBottomRightTile ||
            NewType == TileType.RoomBottomTile ||
            NewType == TileType.RoomLeftTile ||
            NewType == TileType.RoomRightTile ||
            NewType == TileType.RoomTopLeftTile ||
            NewType == TileType.RoomTopRightTile ||
            NewType == TileType.RoomTopTile)
            gameObject.tag = "Wall";
        else if (NewType == TileType.RoomMiddleTile)
            gameObject.tag = "Floor";
        else
            gameObject.tag = "Empty";
    }

    //Toggles the tiles box collider based on its new type
    private void ToggleCollider(TileType NewType)
    {
        bool SolidTile = NewType == TileType.RoomTopTile ||
            NewType == TileType.RoomTopRightTile ||
            NewType == TileType.RoomRightTile ||
            NewType == TileType.RoomBottomRightTile ||
            NewType == TileType.RoomBottomTile ||
            NewType == TileType.RoomBottomLeftTile ||
            NewType == TileType.RoomLeftTile ||
            NewType == TileType.RoomTopLeftTile;// ||
            //NewType == TileType.RoomDoor;
        ToggleCollider(SolidTile);
    }
}

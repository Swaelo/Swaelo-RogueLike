// ================================================================================================================================
// File:        DungeonTile.cs
// Description:	Struct holding data for a single tile in the dungeon grid layout
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonTile
{
    public Vector2 GridPos; //This tiles coordinates in the dungeon grid layout
    public Vector3 WorldPos;    //This tiles position in the game world
    public DungeonTileType TileType = DungeonTileType.Void; //Tracks which type of tile this is
    public GameObject TileObject = null;    //Gameobject used to represent the tile ingame
    public bool TileInitialised = false;    //Tracks if the tile has been setup yet

    //Constructor
    public DungeonTile(Vector2 GridPos, Vector3 WorldPos)
    {
        this.GridPos = GridPos;
        this.WorldPos = WorldPos;
    }

    //Initialised the tile gameobject if its not done yet, then sets its type and sprite to render with
    public void SetType(DungeonTileType TileType)
    {
        //Store the new tile type
        this.TileType = TileType;

        //Initialize the tile object if it hasnt been done yet
        if(!TileInitialised)
        {
            TileInitialised = true;
            TileObject = GameObject.Instantiate(DungeonGenerator.Instance.DungeonTilePrefab, WorldPos, Quaternion.identity);
        }

        //Update the tiles sprite to match the new type given
        foreach(DungeonTileSprites.DungeonTileSprite TileSprite in DungeonTileSprites.Instance.TileSprites)
        {
            if(TileSprite.Type == TileType)
            {
                TileObject.SendMessage("UpdateSprite", TileSprite.Sprite);
                TileObject.SendMessage("ToggleCollider", TileType);
                break;
            }
        }

        //Update the tiles tag
        if (TileType == DungeonTileType.Door)
            TileObject.tag = "Door";
        else if (TileType == DungeonTileType.BottomLeftWall ||
            TileType == DungeonTileType.BottomRightWall ||
            TileType == DungeonTileType.BottomWall ||
            TileType == DungeonTileType.LeftWall ||
            TileType == DungeonTileType.RightWall ||
            TileType == DungeonTileType.TopLeftWall ||
            TileType == DungeonTileType.TopRightWall ||
            TileType == DungeonTileType.TopWall)
            TileObject.tag = "Wall";
        else if (TileType == DungeonTileType.Floor)
            TileObject.tag = "Floor";
        else
            TileObject.tag = "Empty";
    }
}

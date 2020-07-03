// ================================================================================================================================
// File:        DungeonTileSprites.cs
// Description:	Stores the sprites used for each different type of dungeon tile, and makes them easily accessible
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonTileSprites : MonoBehaviour
{
    //Singleton instance for easy access
    public static DungeonTileSprites Instance = null;
    private void Awake() { Instance = this; }

    //All the tile sprites matched up with their tile types
    [System.Serializable]
    public struct DungeonTileSprite
    {
        public DungeonTileType Type;
        public Sprite Sprite;
    }
    public DungeonTileSprite[] TileSprites;
}

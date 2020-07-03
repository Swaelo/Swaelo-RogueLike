// ================================================================================================================================
// File:        DungeonTileManager.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DungeonTileManager : MonoBehaviour
{
    public SpriteRenderer Renderer; //Component used to render the tile ingame
    public BoxCollider2D Collider;  //Body used to detect physics collision events

    //Updates the sprite used for rendering
    public void UpdateSprite(Sprite NewSprite)
    {
        Renderer.sprite = NewSprite;
    }

    //Toggles the tiles physics collider based on its new tile type
    private void SetCollider(bool Activate)
    {
        Collider.enabled = Activate;
    }
    public void ToggleCollider(DungeonTileType NewType)
    {
        bool SolidTile =
            NewType == DungeonTileType.TopWall ||
            NewType == DungeonTileType.TopRightWall ||
            NewType == DungeonTileType.RightWall ||
            NewType == DungeonTileType.BottomRightWall ||
            NewType == DungeonTileType.BottomWall ||
            NewType == DungeonTileType.BottomLeftWall ||
            NewType == DungeonTileType.LeftWall ||
            NewType == DungeonTileType.TopLeftWall;
        SetCollider(SolidTile);
    }
}

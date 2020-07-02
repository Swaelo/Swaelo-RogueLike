// ================================================================================================================================
// File:        NewDungeonTile.cs
// Description:	Defines a single tile in the dungeon grid layout
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class NewDungeonTile : MonoBehaviour
{
    public Vector2 GridPos; //This tiles coordinates in the dungeon grid layout

    //Gives the tile its dungeon grid layout coordinates
    public void SetGridPos(Vector2 GridPos)
    {
        this.GridPos = GridPos;
    }
}

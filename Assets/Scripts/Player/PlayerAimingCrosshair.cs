// ================================================================================================================================
// File:        PlayerAimingCrosshair.cs
// Description:	Replaces the mouse cursor with a crosshair used for aiming the players weapon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerAimingCrosshair : MonoBehaviour
{
    public Texture2D CrosshairTexture;  //Custom crosshair texture which will replace the default mouse cursor
    private Vector2 CursorOffset = new Vector2(7.5f, 7.5f); //Texture offset from cursor location

    private void Start()
    {
        //Set the new custom mouse cursor
        Cursor.SetCursor(CrosshairTexture, CursorOffset, CursorMode.Auto);
    }
}

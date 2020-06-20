// ================================================================================================================================
// File:        CursorLocation.cs
// Description:	Keeps track of the mouse cursors location inside the game world and makes it available for other scripts to use
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CursorLocation : MonoBehaviour
{
    //Singleton Instance
    public static CursorLocation Instance = null;
    private void Awake() { Instance = this; }

    //Cursor location is tracked and kept here for public access
    private Vector3 CurrentLocation = Vector3.zero;
    public Vector3 Get() { return CurrentLocation; }

    private void Update()
    {
        //Cast ray to find cursors location, then update public variable
        Ray CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit RayHit;
        if (Physics.Raycast(CameraRay, out RayHit, Mathf.Infinity))
            CurrentLocation = RayHit.point;
    }
}
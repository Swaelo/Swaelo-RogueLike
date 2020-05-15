// ================================================================================================================================
// File:        CameraMovement.cs
// Description:	Moves the camera with WASD and zoom it with scrollwheel
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public int MoveSpeed = 5;   //How fast the camera moves around
    public int ZoomSpeed = 15;  //How fast the camera zooms
    private float XPos = 0f;   //Cameras X Position
    private float YPos = 0f;   //Cameras Y Position
    private float Zoom = 5;    //Cameras zoom level

    private void Update()
    {
        //Adjust X/Y pos and Zoom level
        XPos += Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;
        YPos += Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
        Zoom -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;

        //Reposition camera with new values
        transform.position = new Vector3(XPos, YPos, -10f);
        GetComponent<Camera>().orthographicSize = Zoom;
    }
}

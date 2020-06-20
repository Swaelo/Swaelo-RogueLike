// ================================================================================================================================
// File:        PlayerMove.cs
// Description:	Allows the player to move around
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Animator AnimationController;    //Used to control the players animations
    public float MoveSpeed = 1.5f;  //How fast the player moves
    private Vector3 PreviousPos = Vector3.zero;    //Previous frames position

    //Directions the player is able to face
    private Vector2 NorthDirection = new Vector2(0f, -1f);
    private Vector2 EastDirection = new Vector2(-1f, 0f);
    private Vector2 SouthDirection = new Vector2(0f, 1f);
    private Vector2 WestDirection = new Vector2(1f, 0f);

    //Keep track of which direction the player is currently facing
    public Direction FacingDirection = Direction.East;

    private void Awake() { PreviousPos = transform.position; }  //Store initial location as previous

    private void Update()
    {
        MovePlayer();
        AnimatePlayer();
        PreviousPos = transform.position;
    }

    //Updates the player position based on user input
    private void MovePlayer()
    {
        //Create a new movement vector based on user input
        Vector3 MovementVector = new Vector3
            (Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"), 0f);

        //Apply this to update the players position
        transform.position += MovementVector * MoveSpeed * Time.deltaTime;
    }

    //Controls animations / sprite flipping so the character is drawn properly
    private void AnimatePlayer()
    {
        //Check if the player is moving, and give this info to the animation controller
        bool IsMoving = transform.position != PreviousPos;
        AnimationController.SetBool("IsMoving", IsMoving);

        //Find current direction vector pointing from the player towards the crosshair
        Vector3 CursorPos = CursorLocation.Instance.Get();
        Vector3 PlayerToCrosshairDirection = Vector3.Normalize(transform.position - CursorPos);

        //Measure the crosshair direction against the directions the player is able to face
        float NorthAngle = Vector3.Angle(PlayerToCrosshairDirection, NorthDirection);
        float EastAngle = Vector3.Angle(PlayerToCrosshairDirection, EastDirection);
        float SouthAngle = Vector3.Angle(PlayerToCrosshairDirection, SouthDirection);
        float WestAngle = Vector3.Angle(PlayerToCrosshairDirection, WestDirection);

        //Face the player north/south when moving up or down
        if (NorthAngle <= EastAngle && NorthAngle <= SouthAngle && NorthAngle <= WestAngle)
        {
            //Face the player north
            FacingDirection = Direction.North;
            AnimationController.SetBool("FacingSide", false);
            AnimationController.SetBool("FacingForward", false);
            AnimationController.SetBool("FacingBack", true);
        }
        else if (SouthAngle <= NorthAngle && SouthAngle <= EastAngle && SouthAngle <= WestAngle)
        {
            //Face the player south
            FacingDirection = Direction.South;
            AnimationController.SetBool("FacingSide", false);
            AnimationController.SetBool("FacingBack", false);
            AnimationController.SetBool("FacingForward", true);
        }
        else
        {
            //Otherwise face the player sideways
            AnimationController.SetBool("FacingForward", false);
            AnimationController.SetBool("FacingBack", false);
            AnimationController.SetBool("FacingSide", true);

            //And flip their sprite when moving west
            bool FacingWest = WestAngle <= NorthAngle && WestAngle <= EastAngle && WestAngle <= SouthAngle;
            FacingDirection = FacingWest ? Direction.West : Direction.East;
            Renderer.flipX = FacingWest;
        }
    }
}

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

        //Change between front/back/side view modes depending on which direction the player is moving
        if(IsMoving)
        {
            //Measure distance travelled in each direction since last frame
            float VertMovement = Mathf.Abs(transform.position.y - PreviousPos.y);
            float HoriMovement = Mathf.Abs(transform.position.x - PreviousPos.x);

            //Set front/back view when moving vertically
            if(VertMovement > HoriMovement)
            {
                //Moving Up
                if (transform.position.y > PreviousPos.y)
                {
                    AnimationController.SetBool("FacingSide", false);
                    AnimationController.SetBool("FacingForward", false);
                    AnimationController.SetBool("FacingBack", true);
                }
                //Moving Down
                else if(transform.position.y < PreviousPos.y)
                {
                    AnimationController.SetBool("FacingSide", false);
                    AnimationController.SetBool("FacingBack", false);
                    AnimationController.SetBool("FacingForward", true);
                }
            }
            //Set side view when moving horizontally
            else
            {
                AnimationController.SetBool("FacingForward", false);
                AnimationController.SetBool("FacingBack", false);
                AnimationController.SetBool("FacingSide", true);

                //Flip sprites vertically when moving to the left
                if (transform.position.x < PreviousPos.x)
                    Renderer.flipX = true;
                else if (transform.position.x > PreviousPos.x)
                    Renderer.flipX = false;
            }
        }
    }
}

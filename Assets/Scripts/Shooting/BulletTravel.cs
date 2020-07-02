// ================================================================================================================================
// File:        BulletTravel.cs
// Description:	Applies movement to the bullet every frame and handles all collision events
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class BulletTravel : MonoBehaviour
{
    public BulletFade BulletFade;   //Used to destroy the bullet once its sat on the ground for a short moment
    public float TravelSpeed = 3.5f;    //How fast the bullet travels across the screen
    private Vector3 MovementDirection;  //Which direction this bullet is travelling in
    private bool DirectionSet = false;  //Tracks whether or not the bullet has been initialized properly

    //Gives the bullet its travel direction and allows it to begin moving
    public void InitializeBullet(Vector3 TravelDirection)
    {
        DirectionSet = true;
        MovementDirection = TravelDirection;
    }

    //Keeps the bullet moving forward every frame
    private void Update()
    {
        if(DirectionSet)
        {
            Vector3 NewPosition = transform.position + MovementDirection * TravelSpeed * Time.deltaTime;
            NewPosition.z = 0f;
            transform.position = NewPosition;
        }
    }

    //Detects and handles collision events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy the projectile if it comes into contact with any parts of the environment, like walls, doors etc.
        if (collision.transform.CompareTag("Wall"))
            DropBullet();
        else if (collision.transform.CompareTag("Door"))
            DropBullet();
    }

    //Drops the bullet onto the ground and triggers the BulletFade to destroy it a few moments later
    private void DropBullet()
    {
        BulletFade.SetFade(1.5f);
        Destroy(this);
    }
}

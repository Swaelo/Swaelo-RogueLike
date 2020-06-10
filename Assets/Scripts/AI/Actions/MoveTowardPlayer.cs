// ================================================================================================================================
// File:        MoveTowardPlayer.cs
// Description:	Moves the current entity towards the player character
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class MoveTowardPlayer : MonoBehaviour
{
    public float MoveSpeed = 1f;    //How fast to move towards the player
    public NodeStates PerformMoveTowardPlayer()
    {
        //Get the current direction from this entity to the player
        Vector3 PlayerDirection = Vector3.Normalize(Player.Instance.transform.position - transform.position);
        //Move in this direction
        transform.position += PlayerDirection * MoveSpeed * Time.deltaTime;
        return NodeStates.SUCCESS;
    }
}

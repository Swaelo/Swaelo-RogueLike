// ================================================================================================================================
// File:        PlayerDistanceCheck.cs
// Description:	Checks if the distance between this entity and the player is below a certain range
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerDistanceCheck : MonoBehaviour
{
    public float MaxDistance = 1f;  //Max distance allowed between the entity and the player

    //Returns success if the distance to the player is <= the max value, otherwise returns failure
    public NodeStates PerformPlayerDistanceCheck()
    {
        float PlayerDistance = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (PlayerDistance <= MaxDistance)
        {
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }
}

// ================================================================================================================================
// File:        AttackPlayer.cs
// Description:	Attacks the player if the cooldown isnt active
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    public float AttackCooldown = 1f;   //How long the entity must wait between attacks
    private bool AttackOnCooldown = false;  //Tracks when the entity isnt able to attack again yet
    private void ResetAttackCooldown() { AttackOnCooldown = false; } //Resets the attack cooldown
    public NodeStates PerformAttackPlayer()
    {
        if(!AttackOnCooldown)
        {
            AttackOnCooldown = true;
            Invoke("ResetAttackCooldown", AttackCooldown);
        }
        return NodeStates.SUCCESS;
    }
}

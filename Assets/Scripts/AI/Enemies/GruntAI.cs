// ================================================================================================================================
// File:        GruntAI.cs
// Description:	Implements AI for the Grunt enemy, seeks the player and attacks once it gets close enough
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class GruntAI : MonoBehaviour
{
    //Root level node
    SelectorNode RootNode;
    //Roots left child, attacks player if they are close enough
    SequenceNode AttackIfInRange;
    ActionNode IsPlayerInRange;
    ActionNode AttackPlayer;
    private float AttackRange = .5f;   //How close the player must be to attack
    private bool AttackOnCooldown = false;
    private float AttackCooldown = 1f;
    //Roots right child, goes here if the player is too far away to attack
    ActionNode SeekPlayer;
    private float MoveSpeed = 1.35f;    //How fast the grunt seeks towards the player

    private void Start()
    {
        //Instantiate behaviour tree nodes from the bottom up, and assign the children in that order
        IsPlayerInRange = new ActionNode(RangeCheckAction);
        AttackPlayer = new ActionNode(AttackPlayerAction);
        List<Node> SequenceChildren = new List<Node>();
        SequenceChildren.Add(IsPlayerInRange);
        SequenceChildren.Add(AttackPlayer);
        AttackIfInRange = new SequenceNode(SequenceChildren);

        SeekPlayer = new ActionNode(SeekPlayerAction);
        List<Node> RootChildren = new List<Node>();
        RootChildren.Add(AttackIfInRange);
        RootChildren.Add(SeekPlayer);
        RootNode = new SelectorNode(RootChildren);
    }

    private void Update()
    {
        RootNode.Evaluate();
    }

    private void ResetAttack()
    {
        AttackOnCooldown = false;
    }

    //Player distance check function for the action node
    private NodeStates RangeCheckAction()
    {
        //Check the distance between player and enemy
        float PlayerDistance = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (PlayerDistance <= AttackRange)
            return NodeStates.SUCCESS;
        return NodeStates.FAILURE;
    }

    //Player attack function for the attack player action node
    private NodeStates AttackPlayerAction()
    {
        if(!AttackOnCooldown)
        {
            AttackOnCooldown = true;
            Invoke("ResetAttack", AttackCooldown);
        }
        return NodeStates.SUCCESS;
    }

    //Player following function for the seek player action node
    private NodeStates SeekPlayerAction()
    {
        //Get the players direction
        Vector3 PlayerDirection = Vector3.Normalize(Player.Instance.transform.position - transform.position);
        //Move in this direction to get closer to the player
        transform.position += PlayerDirection * MoveSpeed * Time.deltaTime;
        return NodeStates.SUCCESS;
    }
}

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
    //Roots right child, goes here if the player is too far away to attack
    ActionNode SeekPlayer;

    private void Start()
    {
        //Instantiate behaviour tree nodes from the bottom up, and assign the children in that order
        IsPlayerInRange = new ActionNode(GetComponent<PlayerDistanceCheck>().PerformPlayerDistanceCheck);
        AttackPlayer = new ActionNode(GetComponent<AttackPlayer>().PerformAttackPlayer);
        List<Node> SequenceChildren = new List<Node>();
        SequenceChildren.Add(IsPlayerInRange);
        SequenceChildren.Add(AttackPlayer);
        SeekPlayer = new ActionNode(GetComponent<MoveTowardPlayer>().PerformMoveTowardPlayer);
        List<Node> RootChildren = new List<Node>();
        RootChildren.Add(AttackIfInRange);
        RootChildren.Add(SeekPlayer);
    }

    private void Update()
    {
        RootNode.Evaluate();
    }
}

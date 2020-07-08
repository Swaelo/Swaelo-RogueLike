// ================================================================================================================================
// File:        GruntAI.cs
// Description:	Implements AI for the Grunt enemy, seeks the player and attacks once it gets close enough
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerDistanceCheck))]
[RequireComponent(typeof(AttackPlayer))]
[RequireComponent(typeof(MoveTowardPlayer))]
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
        IsPlayerInRange = new ActionNode(GetComponent<PlayerDistanceCheck>().PerformPlayerDistanceCheck);
        AttackPlayer = new ActionNode(GetComponent<AttackPlayer>().PerformAttackPlayer);
        List<Node> SequenceChildren = new List<Node>();
        SequenceChildren.Add(IsPlayerInRange);
        SequenceChildren.Add(AttackPlayer);
        AttackIfInRange = new SequenceNode(SequenceChildren);
        SeekPlayer = new ActionNode(GetComponent<MoveTowardPlayer>().PerformMoveTowardPlayer);
        List<Node> RootChildren = new List<Node>();
        RootChildren.Add(AttackIfInRange);
        RootChildren.Add(SeekPlayer);
        RootNode = new SelectorNode(RootChildren);
    }

    private void Update()
    {
        RootNode.Evaluate();
    }
}

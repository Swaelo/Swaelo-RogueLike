// ================================================================================================================================
// File:        Node.cs
// Description:	Base behaviour tree node object for AI behaviour trees, design taken from https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Node
{
    public delegate NodeStates NodeReturn();    //Returns the state of the node
    protected NodeStates State; //The current state of the node
    public NodeStates NodeState { get { return State; } }   //Node state getter
    public Node() { }   //Constructor
    public abstract NodeStates Evaluate();  //Implemented classes use this method to evaluate the desired set of conditions
}

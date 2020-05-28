// ================================================================================================================================
// File:        SequenceNode.cs
// Description:	Behaviour tree sequence node, design taken from public Shub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;

public class SequenceNode : Node
{
    protected List<Node> Nodes = new List<Node>();  //Children nodes belonging to this sequence
    public SequenceNode(List<Node> Nodes)
    {
        this.Nodes = ListFunctions.Copy(Nodes);
    }

    //If any child node returns a failure, the entire node fails
    //Once all nodes return a success, the node reports a success
    public override NodeStates Evaluate()
    {
        bool AnyChildRunning = false;

        foreach(Node Node in Nodes)
        {
            switch(Node.Evaluate())
            {
                case (NodeStates.FAILURE):
                    State = NodeStates.FAILURE;
                    return State;
                case (NodeStates.SUCCESS):
                    continue;
                case (NodeStates.RUNNING):
                    AnyChildRunning = true;
                    continue;
                default:
                    State = NodeStates.SUCCESS;
                    return State;
            }
        }
        State = AnyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return State;
    }
}

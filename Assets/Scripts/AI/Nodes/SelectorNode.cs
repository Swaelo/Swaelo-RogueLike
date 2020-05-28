// ================================================================================================================================
// File:        SelectorNode.cs
// Description:	Behaviour tree selector node, design taken from https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;

public class SelectorNode : Node
{
    protected List<Node> Nodes = new List<Node>();  //The child nodes for this selector
    public SelectorNode(List<Node> Nodes)
    {
        this.Nodes = ListFunctions.Copy(Nodes);
    }

    //If any of the children reports a success, the selector will immediately report a success upwards
    //If all children fail, it will report a failure instead
    public override NodeStates Evaluate()
    {
        foreach(Node Node in Nodes)
        {
            switch(Node.Evaluate())
            {
                case (NodeStates.FAILURE):
                    continue;
                case (NodeStates.SUCCESS):
                    State = NodeStates.SUCCESS;
                    return State;
                case (NodeStates.RUNNING):
                    State = NodeStates.RUNNING;
                    return State;
                default:
                    continue;
            }
        }
        State = NodeStates.FAILURE;
        return State;
    }
}

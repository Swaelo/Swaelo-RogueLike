// ================================================================================================================================
// File:        InverterNode.cs
// Description:	Behaviour tree inverter node, design taken from https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

public class InverterNode : Node
{
    Node Node;  //Child node to evaluate
    public InverterNode(Node Node) { this.Node = Node; }    //Constructor

    //Reports a success if the child fails, and a failure if the child succeeds.
    //Running will report as running
    public override NodeStates Evaluate()
    {
        switch(Node.Evaluate())
        {
            case (NodeStates.FAILURE):
                State = NodeStates.SUCCESS;
                return State;
            case (NodeStates.SUCCESS):
                State = NodeStates.FAILURE;
                return State;
            case (NodeStates.RUNNING):
                State = NodeStates.RUNNING;
                return State;
        }
        State = NodeStates.SUCCESS;
        return State;
    }
}

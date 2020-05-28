// ================================================================================================================================
// File:        ActionNode.cs
// Description:	Behaviour tree action node, design taken from https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

public class ActionNode : Node
{
    public delegate NodeStates ActionNodeDelegate();   //Method signature for the action logic
    private ActionNodeDelegate Action;  //The delegate that is called to evaluate this node
    public ActionNode(ActionNodeDelegate Action) { this.Action = Action; }  //Action Node logic must be passed in the form of a delegate

    //Evaluated the node using the passed in delegate and reports the resulting state
    public override NodeStates Evaluate()
    {
        switch(Action())
        {
            case (NodeStates.SUCCESS):
                State = NodeStates.SUCCESS;
                return State;
            case (NodeStates.FAILURE):
                State = NodeStates.FAILURE;
                return State;
            case (NodeStates.RUNNING):
                State = NodeStates.RUNNING;
                return State;
            default:
                State = NodeStates.FAILURE;
                return State;
        }
    }
}

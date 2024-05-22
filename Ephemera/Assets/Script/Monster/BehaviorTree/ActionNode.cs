public class ActionNode : Node
{
    public delegate State ActionNodeDelegate();
    private ActionNodeDelegate action;

    public ActionNode(ActionNodeDelegate action)
    {
        this.action = action;
    }

    public override State Evaluate()
    {
        return action();
    }
}
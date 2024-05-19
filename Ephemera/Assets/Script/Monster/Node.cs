public abstract class Node
{
    public enum State
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    protected State state;
    public State NodeState { get { return state; } }

    public abstract State Evaluate();
}
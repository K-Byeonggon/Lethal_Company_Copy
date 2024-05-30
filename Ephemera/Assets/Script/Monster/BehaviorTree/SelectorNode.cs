using System.Collections.Generic;

//children의 Node중 하나만 실행해야 할때 사용
public class SelectorNode : CompositeNode
{
    public SelectorNode(List<Node> children) : base(children) { }

    public override State Evaluate()
    {
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case State.RUNNING:
                    state = State.RUNNING;
                    return state;
                case State.SUCCESS:
                    state = State.SUCCESS;
                    return state;
                case State.FAILURE:
                    continue;
            }
        }
        state = State.FAILURE;
        return state;
    }
}
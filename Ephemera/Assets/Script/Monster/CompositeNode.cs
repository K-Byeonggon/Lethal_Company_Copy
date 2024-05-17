using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    protected List<Node> children = new List<Node>();

    public CompositeNode(List<Node> children)
    {
        this.children = children;
    }
}
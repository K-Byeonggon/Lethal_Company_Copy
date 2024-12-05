using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    protected List<Node> children = new List<Node>();
    protected int runningNodeIndex = -1;    //Running상태를 캐싱하기 위한 인덱스

    //생성자에서 자식 노드들 초기화
    public CompositeNode(List<Node> children)
    {
        this.children = children;
    }
}
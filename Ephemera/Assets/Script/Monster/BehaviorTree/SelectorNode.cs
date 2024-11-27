using System.Collections.Generic;

//children의 Node중 하나만 실행해야 할때 사용
public class SelectorNode : CompositeNode
{
    //부모인 CompositeNode의 생성자 호출
    public SelectorNode(List<Node> children) : base(children) { }

    //자식 노드들을 평가하고, 하나라도 성공하면 성공을 반환한다.
    public override State Evaluate()
    {
        int startIndex = runningNodeIndex != -1 ? runningNodeIndex : 0;

        for (int i = startIndex; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case State.RUNNING:
                    runningNodeIndex = i;   //현재 노드를 기억
                    state = State.RUNNING;  //셀렉터도 RUNNING이 된다.
                    return state;
                case State.SUCCESS:
                    runningNodeIndex = -1;  // 성공시 RUNNING 인덱스 초기화
                    state = State.SUCCESS;  //셀렉터가 SUCCESS 반환
                    return state;
                case State.FAILURE:
                    continue;               //실패시 다음 노드 평가
            }
        }

        runningNodeIndex = -1;  //모든 노드 실패시 인덱스 초기화
        state = State.FAILURE;  //셀렉터가 FAILURE 반환
        return state;
    }
}
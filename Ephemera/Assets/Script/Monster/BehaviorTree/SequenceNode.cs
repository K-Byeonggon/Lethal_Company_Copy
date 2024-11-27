using System.Collections.Generic;

//children의 Node를 순서대로 실행해야 할때 사용
public class SequenceNode : CompositeNode
{
    //부모인 CompositeNode의 생성자 호출
    public SequenceNode(List<Node> children) : base(children) { }

    public override State Evaluate()
    {
        int startIndex = runningNodeIndex != -1 ? runningNodeIndex : 0;

        for (int i = startIndex; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case State.RUNNING:
                    runningNodeIndex = i;       //현재 노드를 기억
                    state = State.RUNNING;      //시퀀스도 RUNNING이 된다.
                    return state;
                case State.SUCCESS:
                    continue;                   //성공시 다음 노드 평가
                case State.FAILURE:
                    runningNodeIndex = -1;      //실패시 Running노드 초기화
                    state = State.FAILURE;      //시퀀스가 FAILURE 반환
                    return state;
            }
        }

        runningNodeIndex = -1;      //인덱스 초기화
        state = State.SUCCESS;      //모든 노드가 성공하면 성공 반환
        return state;
    }
}
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        // 게임 시작 후 NavMesh를 베이크
        BakeNavMesh();
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface가 설정되지 않았습니다.");
            return;
        }

        // NavMesh 베이크
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 베이크되었습니다.");
    }

    // NavMesh를 재베이크하고 싶을 때 이 메소드를 호출
    public void RebuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 재베이크되었습니다.");
    }
}
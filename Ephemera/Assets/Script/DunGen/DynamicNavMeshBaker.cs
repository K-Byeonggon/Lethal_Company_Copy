using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        // ���� ���� �� NavMesh�� ����ũ
        BakeNavMesh();
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // NavMesh ����ũ
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� ����ũ�Ǿ����ϴ�.");
    }

    // NavMesh�� �纣��ũ�ϰ� ���� �� �� �޼ҵ带 ȣ��
    public void RebuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� �纣��ũ�Ǿ����ϴ�.");
    }
}
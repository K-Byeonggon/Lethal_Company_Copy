using UnityEngine;
using UnityEngine.AI;
using DunGen;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public GameObject dungeonGeneratorObject;

    private DungeonGenerator dungeonGenerator;

    void Start()
    {
        if (dungeonGeneratorObject == null)
        {
            Debug.LogError("DungeonGenerator ������Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }

        dungeonGenerator = dungeonGeneratorObject.GetComponent<RuntimeDungeon>().Generator;
        if (dungeonGenerator == null)
        {
            Debug.LogError("DungeonGenerator ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // DunGen�� ���� ���� ���� ���� �̺�Ʈ ����
        dungeonGenerator.OnGenerationStatusChanged += OnGenerationStatusChanged;
    }

    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnGenerationStatusChanged -= OnGenerationStatusChanged;
        }
    }

    private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
    {
        // ���� ������ �Ϸ�Ǿ��� �� NavMesh ����ũ
        if (status == GenerationStatus.Complete)
        {
            BakeNavMesh();
        }
    }

    private void BakeNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // NavMesh ����ũ
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� ������ ����ũ�Ǿ����ϴ�.");
    }
}
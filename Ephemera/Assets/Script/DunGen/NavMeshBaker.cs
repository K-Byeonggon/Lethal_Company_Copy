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
            Debug.LogError("DungeonGenerator 오브젝트가 설정되지 않았습니다.");
            return;
        }

        dungeonGenerator = dungeonGeneratorObject.GetComponent<RuntimeDungeon>().Generator;
        if (dungeonGenerator == null)
        {
            Debug.LogError("DungeonGenerator 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // DunGen의 던전 생성 상태 변경 이벤트 구독
        dungeonGenerator.OnGenerationStatusChanged += OnGenerationStatusChanged;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnGenerationStatusChanged -= OnGenerationStatusChanged;
        }
    }

    private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
    {
        // 던전 생성이 완료되었을 때 NavMesh 베이크
        if (status == GenerationStatus.Complete)
        {
            BakeNavMesh();
        }
    }

    private void BakeNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface가 설정되지 않았습니다.");
            return;
        }

        // NavMesh 베이크
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 던전에 베이크되었습니다.");
    }
}
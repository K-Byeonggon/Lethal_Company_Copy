using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DunGen.Adapters
{
    [AddComponentMenu("DunGen/NavMesh/Unity NavMesh Generator")]
    public class UnityNavMeshAdapter : MonoBehaviour
    {
        public int AgentTypeID = 0;

        private NavMeshSurface navMeshSurface;

        void Awake()
        {
            // NavMeshSurface ������Ʈ�� ã�ų� ����
            navMeshSurface = GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
            }

            navMeshSurface.agentTypeID = AgentTypeID;
        }

        public void Generate(Dungeon dungeon)
        {
            // NavMeshSurface ����
            navMeshSurface.transform.position = dungeon.Bounds.center;
            navMeshSurface.transform.localScale = dungeon.Bounds.size;
#if UNITY_EDITOR
            // NavMesh ���� ����
            StartCoroutine(GenerateNavMesh());
#endif

        }

#if UNITY_EDITOR
        private IEnumerator GenerateNavMesh()
        {
            // �׺���̼� �޽� ����
            navMeshSurface.BuildNavMesh();
            // ���尡 �Ϸ�� ������ ��ٸ�
            while (UnityEditor.AI.NavMeshBuilder.isRunning)
            {
                yield return null;
            }

            Debug.Log("NavMesh generation completed");
        }
#endif
    }
}
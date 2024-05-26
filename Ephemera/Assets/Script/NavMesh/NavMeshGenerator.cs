using DunGen;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    public void BakeNavMesh()
    {
        Invoke("StartBake", 5f);
    }
    public void StartBake()
    {
        StartCoroutine(BuildNavMesh(navMeshSurface, new Bounds(Vector3.zero, new Vector3(1000,1500,1000))));
    }
    
    private IEnumerator BuildNavMesh(NavMeshSurface surface, Bounds bounds)
    {
        // �׺�޽� �����͸� �ʱ�ȭ�մϴ�.
        navMeshSurface.RemoveData();
        yield return null;  // ���� �����ӱ��� ���
        var data = NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), new List<NavMeshBuildSource>(), new Bounds(), surface.transform.position, surface.transform.rotation);
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(bounds, surface.layerMask, NavMeshCollectGeometry.PhysicsColliders, surface.defaultArea, new List<NavMeshBuildMarkup>(), sources);

        float timer = Time.unscaledTime;
        AsyncOperation async = NavMeshBuilder.UpdateNavMeshDataAsync(data, surface.GetBuildSettings(), sources, bounds);
        while (!async.isDone)
        {
            Debug.Log("Update Progress: " + async.progress);
            yield return null;
        }
        Debug.Log("Nav Mesh Time: " + (Time.unscaledTime - timer));

        surface.navMeshData = data;
        surface.AddData();

        GameManager.Instance.SpawnItem();
        GameManager.Instance.SpawnMonster();
        GameManager.Instance.OnServerDoorLinkSequence();
    }
}
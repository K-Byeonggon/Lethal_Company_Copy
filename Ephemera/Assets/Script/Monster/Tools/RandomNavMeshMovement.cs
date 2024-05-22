using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshMovement : MonoBehaviour
{
    public float wanderRadius = 10f; // 몬스터가 이동할 수 있는 최대 반경
    public float wanderInterval = 5f; // 몬스터가 새로운 목적지로 이동하는 간격
    public Transform nest;

    private NavMeshAgent agent;
    private float timer;

    /*
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(nest.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }*/

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;

        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        //Debug.Log(navHit.position);
        return navHit.position;
    }

    public static Vector3 NavAwayFromPlayer(Vector3 origin, Vector3 player, float moveDistance)
    {
        // 플레이어와의 방향 벡터 계산
        Vector3 directionFromPlayer = origin - player;
        directionFromPlayer.Normalize();

        // 목표 위치 계산
        Vector3 targetPosition = origin + directionFromPlayer * moveDistance;

        // 목표 위치가 NavMesh 위에 있는지 확인하고 이동
        NavMeshHit navHit;
        NavMesh.SamplePosition(targetPosition, out navHit, moveDistance, NavMesh.AllAreas);

        return navHit.position;

    }

    public static float GetAngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        // 벡터를 정규화하여 단위 벡터로 만듭니다.
        v1.Normalize();
        v2.Normalize();

        // 두 벡터의 내적을 계산합니다.
        float dotProduct = Vector3.Dot(v1, v2);

        // 내적의 값이 -1과 1 사이에 있도록 제한합니다. (수치적 오차를 방지하기 위해)
        dotProduct = Mathf.Clamp(dotProduct, -1.0f, 1.0f);

        // 내적을 이용하여 두 벡터 사이의 각도를 계산합니다.
        float angleInRadians = Mathf.Acos(dotProduct);

        // 각도를 라디안에서 도(degree) 단위로 변환합니다.
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        Debug.Log("각도: " + angleInDegrees);
        return angleInDegrees;
    }
}

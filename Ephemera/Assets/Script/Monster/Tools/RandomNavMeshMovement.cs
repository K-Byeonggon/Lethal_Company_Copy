using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshMovement : MonoBehaviour
{
    public float wanderRadius = 10f; // ���Ͱ� �̵��� �� �ִ� �ִ� �ݰ�
    public float wanderInterval = 5f; // ���Ͱ� ���ο� �������� �̵��ϴ� ����
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
        // �÷��̾���� ���� ���� ���
        Vector3 directionFromPlayer = origin - player;
        directionFromPlayer.Normalize();

        // ��ǥ ��ġ ���
        Vector3 targetPosition = origin + directionFromPlayer * moveDistance;

        // ��ǥ ��ġ�� NavMesh ���� �ִ��� Ȯ���ϰ� �̵�
        NavMeshHit navHit;
        NavMesh.SamplePosition(targetPosition, out navHit, moveDistance, NavMesh.AllAreas);

        return navHit.position;

    }

    public static float GetAngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        // ���͸� ����ȭ�Ͽ� ���� ���ͷ� ����ϴ�.
        v1.Normalize();
        v2.Normalize();

        // �� ������ ������ ����մϴ�.
        float dotProduct = Vector3.Dot(v1, v2);

        // ������ ���� -1�� 1 ���̿� �ֵ��� �����մϴ�. (��ġ�� ������ �����ϱ� ����)
        dotProduct = Mathf.Clamp(dotProduct, -1.0f, 1.0f);

        // ������ �̿��Ͽ� �� ���� ������ ������ ����մϴ�.
        float angleInRadians = Mathf.Acos(dotProduct);

        // ������ ���ȿ��� ��(degree) ������ ��ȯ�մϴ�.
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        Debug.Log("����: " + angleInDegrees);
        return angleInDegrees;
    }
}

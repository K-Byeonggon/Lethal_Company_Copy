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
}

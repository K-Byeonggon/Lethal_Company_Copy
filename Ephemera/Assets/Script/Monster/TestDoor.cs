using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDoor : MonoBehaviour
{
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] Transform destination;
    

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.SetDestination(destination.position);
        Debug.Log(navMeshAgent.destination);
    }

    private void Update()
    {
        Debug.Log(navMeshAgent.destination);
    }
}

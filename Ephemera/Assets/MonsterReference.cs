using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReference : MonoBehaviour
{
    public static MonsterReference Instance;

    [SerializeField]
    public List<GameObject> monsterList = new List<GameObject>();

    public int PlayerCount => monsterList.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerToList(GameObject monster)
    {
        Instance.monsterList.Add(monster);
    }
}

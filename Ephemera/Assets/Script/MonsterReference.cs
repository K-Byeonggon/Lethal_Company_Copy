using Mirror;
using Mirror.Examples.Basic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReference : MonoBehaviour
{
    public static MonsterReference Instance;

    [SerializeField]
    public List<GameObject> monsterList = new List<GameObject>();

    public int MonsterCount => monsterList.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddMonsterToList(GameObject monster)
    {
        if (monsterList.Contains(monster) == false)
            Instance.monsterList.Add(monster);
    }
    public void DestroyMonster(GameObject monster)
    {
        if(monsterList.Contains(monster))
        {
            monsterList.Remove(monster);
            NetworkIdentity identity = monster.GetComponent<NetworkIdentity>();
            GameManager.Instance.DestroyObject(identity);
        }
    }
    public void DestroyAll()
    {
        for (int i = monsterList.Count - 1; i >= 0; i--)
        {
            GameObject item = monsterList[i];

            if (item != null)
            {
                NetworkIdentity identity = item.GetComponent<NetworkIdentity>();
                GameManager.Instance.DestroyObject(identity);
            }
            monsterList.RemoveAt(i);
        }
        monsterList.Clear();
    }
}

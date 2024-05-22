using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReference : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objects;

    public GameObject GetGameObject(string name)
    {
        foreach (var obj in objects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }
}
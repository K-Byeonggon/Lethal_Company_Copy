using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (GameObject.Find(typeof(T).Name) == null)
            {
                GameObject go = new GameObject() { name = typeof(T).Name };
                if (go.GetComponent<T>() == null)
                {
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}

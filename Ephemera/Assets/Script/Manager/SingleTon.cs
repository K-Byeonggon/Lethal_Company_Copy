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
            if(instance == null)
            {
                GameObject go = GameObject.Find(typeof(T).Name);
                if (go == null)
                {
                    go = new GameObject() { name = typeof(T).Name };
                }
                T t = go.GetComponent<T>();
                if (t == null)
                {
                    t = go.AddComponent<T>();
                }
                instance = t;
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
}

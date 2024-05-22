using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReference : MonoBehaviour
{
    public List<GameObject> prefabs;

    public void LoadToResourceManager()
    {
        prefabs.ForEach(prefab => { ResourceManager.Instance.GetPrefab(prefab.name); });
    }
}

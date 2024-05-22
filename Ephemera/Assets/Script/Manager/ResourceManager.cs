using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Mirror;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : SingleTon<ResourceManager>
{
    Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    private Dictionary<Type, Dictionary<string, ScriptableObject>> _scriptableData = new Dictionary<Type, Dictionary<string, ScriptableObject>>();


    public GameObject GetPrefab(string addressableAssetKey)
    {
        if (!_prefabs.ContainsKey(addressableAssetKey))
        {
            GameObject prefab = DataManager.Instance.LoadObject<GameObject>(addressableAssetKey);
            _prefabs.Add(addressableAssetKey, prefab);
            GameRoomNetworkManager.Instance?.spawnPrefabs.Add(prefab);
        }
        return _prefabs[addressableAssetKey];
    }
    public T GetScriptableData<T>(string addressableAssetKey) where T : ScriptableObject
    {
        if (!_scriptableData.ContainsKey(typeof(T)))
        {
            _scriptableData.Add(typeof(T), new Dictionary<string, ScriptableObject>());
        }
        if (!_scriptableData[typeof(T)].ContainsKey(addressableAssetKey))
        {
            T data = DataManager.Instance.LoadObject<T>(addressableAssetKey);
            _scriptableData[typeof(T)].Add(addressableAssetKey, data);
        }
        return (T)_scriptableData[typeof(T)][addressableAssetKey];
    }
}

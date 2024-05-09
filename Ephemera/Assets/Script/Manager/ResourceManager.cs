using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager : SingleTon<ResourceManager>
{
    Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    private Dictionary<Type, Dictionary<string, ScriptableObject>> _scriptableData = new Dictionary<Type, Dictionary<string, ScriptableObject>>();

    private Dictionary<string, SpriteAtlas> _atlasData = new Dictionary<string, SpriteAtlas>();


    public GameObject GetPrefab(string addressableAssetKey)
    {
        if (!_prefabs.ContainsKey(addressableAssetKey))
        {
            _prefabs.Add(addressableAssetKey, DataManager.Instance.LoadObject<GameObject>(addressableAssetKey));
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
    public SpriteAtlas GetAtlasData(AtlasType atlasType)
    {
        if (!_atlasData.ContainsKey(atlasType.ToString()))
        {
            _atlasData.Add(atlasType.ToString(), DataManager.Instance.LoadObject<SpriteAtlas>(atlasType.ToString()));
        }
        return _atlasData[atlasType.ToString()];
    }
}

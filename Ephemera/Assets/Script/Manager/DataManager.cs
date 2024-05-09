using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : SingleTon<DataManager>
{
    public T LoadObject<T>(string addressableAssetKey)
    {
        return Addressables.LoadAssetAsync<T>(addressableAssetKey).WaitForCompletion();
    }

    public void SaveJsonData<T>(string dataName, T data)
    {
        var json = JsonConvert.SerializeObject(data);
        CreateJsonFile(Application.persistentDataPath, dataName, json);
    }

    public T LoadJsonData<T>(string dataName) where T : class
    {
        return LoadJsonFile<T>(Application.persistentDataPath, dataName);
    }

    public void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
    public T LoadJsonFile<T>(string loadPath, string fileName) where T : class
    {
        var fielPath = string.Format("{0}/{1}.json", loadPath, fileName);
        Debug.Log(fielPath);
        if (File.Exists(fielPath) == false)
        {
            return null;
        }

        FileStream fileStream = new FileStream(fielPath, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}

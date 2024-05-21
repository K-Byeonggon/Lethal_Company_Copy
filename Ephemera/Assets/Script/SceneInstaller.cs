using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstaller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PrefabReference>().LoadToResourceManager();
    }
}

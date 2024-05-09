using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{

    private Dictionary<UIType, RectTransform> uiDic = new Dictionary<UIType, RectTransform>();

    private GameObject eventSystem = null;

    private Dictionary<UIType, GameObject> activatedPopupUI = new Dictionary<UIType, GameObject>();

    GameObject hpUIPrefab;

    public GameObject EventSystem 
    {
        get
        {
            eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = Instantiate(ResourceManager.Instance.GetPrefab("EventSystem"));
                DontDestroyOnLoad(eventSystem);
            }
            return eventSystem;
        }
    }

    public void Init()
    {
        if (eventSystem == null)
        {
            eventSystem = Instantiate(ResourceManager.Instance.GetPrefab("EventSystem"));
            DontDestroyOnLoad(eventSystem);
        }
    }
    public void Clear()
    {
        uiDic.Clear();
        activatedPopupUI.Clear();
    }
}

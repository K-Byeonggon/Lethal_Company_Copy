using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public RectTransform uigroup;

    public void Awake()
    {
        instance = this;
    }

    public GameObject[] itemobj;
    public int[] itemprice;
    public Transform[] itemPos;
    public TextMeshProUGUI text;

    public void Enter()//PlayerMove player)
    {
        uigroup.gameObject.SetActive(true);
        PlayerMove.instance.cameraSpeed = 0;
        CameraMove.instance.mouseSpeed = 0;
    }

    public void Eixt()
    {
        uigroup.gameObject.SetActive(false);
        PlayerMove.instance.cameraSpeed = 3;
        CameraMove.instance.mouseSpeed = 3;
    }

    public void Buy()
    {

    }
}

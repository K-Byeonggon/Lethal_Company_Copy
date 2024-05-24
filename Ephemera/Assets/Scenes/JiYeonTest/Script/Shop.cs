using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using Mirror;

public class Shop : NetworkBehaviour
{
    public static Shop instance;
    public RectTransform uigroup;

    public GameObject[] itemobj;
    public Transform[] itemPos;

    public GameObject Itemparent;

    public Image image;
    public PlayerEx Player;

    public void Awake()
    {
        instance = this;
    }

    public void Enter()//PlayerMove player)
    {
        uigroup.gameObject.SetActive(true);
        GameManager.Instance.localPlayerController.SetCameraSpeed(0);
    }

    public void Eixt()
    {
        uigroup.gameObject.SetActive(false);
        GameManager.Instance.localPlayerController.SetCameraSpeed(3);
        //CameraMove.instance.mouseSpeed = 3;
    }

    [Command]
    public void Buy(int index)
    {
        Vector3 rnaitempos = Vector3.right * Random.Range(-0, 0) + Vector3.forward * Random.Range(-0, 0);
        GameObject gameObject = Instantiate(itemobj[index], itemPos[index].position + rnaitempos, itemPos[index].rotation);
        Item buyItem = gameObject.GetComponent<Item>();

        int currentMonny = GameManager.Instance.CurrentMoney;

        if (currentMonny < buyItem.ItemPrice)
            return;

        GameManager.Instance.OnServerCurrentMoneyChanged(currentMonny - buyItem.ItemPrice);

        if (buyItem == null)
            return;

        //buyItem.itemData = image;

        

        //소지금 부족시 구매 불가 조건 추가

        //Shop ui가 실행되고있을때
        //구매 버튼을 누르면 스폰포인트로 구매한 아이템 소환
    }
}

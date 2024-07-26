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

        GameManager.Instance.CurrentMoney -= buyItem.ItemPrice;

        if (buyItem == null)
            return;

        //buyItem.itemData = image;

        

        //������ ������ ���� �Ұ� ���� �߰�

        //Shop ui�� ����ǰ�������
        //���� ��ư�� ������ ��������Ʈ�� ������ ������ ��ȯ
    }
}

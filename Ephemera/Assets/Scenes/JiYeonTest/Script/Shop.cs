using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
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
        PlayerMove.instance.cameraSpeed = 0;
        //CameraMove.instance.mouseSpeed = 0;
    }

    public void Eixt()
    {
        uigroup.gameObject.SetActive(false);
        PlayerMove.instance.cameraSpeed = 3;
        //CameraMove.instance.mouseSpeed = 3;
    }

    public void Buy(int index)
    {
        Vector3 rnaitempos = Vector3.right * Random.Range(-0, 0) + Vector3.forward * Random.Range(-0, 0);
        GameObject gameObject = Instantiate(itemobj[index], itemPos[index].position + rnaitempos, itemPos[index].rotation);
        Item buyItem = gameObject.GetComponent<Item>();
        Player.coin -= buyItem.itemPrice;

        if (Player.coin < buyItem.itemPrice)
        {
            return;
        }

        if (buyItem == null)
            return;

        buyItem.image = image;

        //������ ������ ���� �Ұ� ���� �߰�

        //Shop ui�� ����ǰ�������
        //���� ��ư�� ������ ��������Ʈ�� ������ ������ ��ȯ
    }
}

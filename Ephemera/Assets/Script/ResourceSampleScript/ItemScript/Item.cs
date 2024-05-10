using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class Item : MonoBehaviour,IUIVisible,IItemUsable,IItemObtainable
{
    [SerializeField] public bool isBothHandGrab;
    private float itemPrice;
    private bool isUsable;

    [SerializeField]
    private Image image;
    [SerializeField]
    private Animator animator;

    private PlayerEx player;
    public void PickDown()
    {
        //animator.SetTrigger("머시기머시기")

        //이 아이템 게임 오브젝트의 월드포지션기준으로 부모관계만 해제
        //player.transform.forward
    }

    public void PickUp()
    {
        //OnGetItem();
        Debug.Log("pickup");

    }

    public void ShowPickupUI()
    {
        image.gameObject.SetActive(true);
    }

    public void UIvisible()
    {
        //Image image = UIManager.Instance.GetUI<Image>("UI이름");
        image.gameObject.SetActive(true);
    }

    public virtual void UseItem()
    {
        
    }

    public void OnGetItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            
        }
    }
}

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

    
    public void PickDown(PlayerEx owner)
    {
        Debug.Log("Pickdown");
        owner.pickedItem.transform.SetParent(null);
        transform.localPosition = owner.transform.forward;
    }

    public void PickUp(PlayerEx owner)//gamemanager
    {
            if (owner != null)
            {
                transform.SetParent(owner.pickedItem);
                transform.position = owner.pickedItem.position;
            }
        
    }

    public void ShowPickupUI()
    {
        image.gameObject.SetActive(true);
    }

    public void UIvisible()
    {
        //Image image = UIManager.Instance.GetUI<Image>("UI¿Ã∏ß");
        image.gameObject.SetActive(true);
    }

    public virtual void UseItem()
    {
        
    }

    public void OnGetItem()
    {
        
    }
}

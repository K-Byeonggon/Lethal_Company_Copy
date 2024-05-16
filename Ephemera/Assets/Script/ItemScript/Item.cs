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
    

    [SerializeField]
    private Image image;
    

    
    public void PickDown(PlayerEx owner)
    {
        Debug.Log("Pickdown");
        transform.SetParent(null);
        transform.position = owner.transform.position+owner.transform.forward;
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
    public float SellItem()
    {
        return itemPrice;
    }
    
}

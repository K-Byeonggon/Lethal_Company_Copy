using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using static UnityEditor.Progress;
public class Item : MonoBehaviour,IUIVisible,IItemUsable,IItemObtainable
{
    [SerializeField] public ItemData itemData;
    [SerializeField] public int itemPrice;
    public bool IsBothHandGrab { get { return itemData.isBothHand; } }
    

    [SerializeField]
    private Image image;

    Collider collider;
    Rigidbody rb;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        itemPrice = itemData.GetRandomPrice();
    }

    public void PickDown(Inventory owner)
    {
        Debug.Log("Pickdown");
        transform.SetParent(null);
        collider.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        transform.position = owner.pickedItem.transform.position + owner.pickedItem.transform.forward;
        rb.AddForce(owner.pickedItem.transform.forward * 5.0f, ForceMode.Impulse); 
    }

    public void PickUp(Inventory owner)//gamemanager
    {
        if (owner != null)
        {
            transform.SetParent(owner.pickedItem);
            //collider.enabled = false;
            collider.enabled = false;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = owner.pickedItem.position;
            transform.rotation = owner.pickedItem.transform.rotation;

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
    public int SellItem()
    {
        return itemPrice;
    }
    
}

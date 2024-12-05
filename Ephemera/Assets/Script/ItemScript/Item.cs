using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Mirror;
public class Item : NetworkBehaviour, IItemUsable, IItemObtainable, IUIVisible
{
    [SerializeField] public ItemData itemData;
    [SyncVar] private int itemPrice;
    public int ItemPrice => itemPrice;
    public Sprite itemSprite => itemData.image;
    public bool IsBothHandGrab => itemData.isBothHand;

    [SerializeField] public Collider itemCollider;
    [SerializeField] public Rigidbody rigid;
    [SerializeField] public List<MeshRenderer> renderers;

    public override void OnStartServer()
    {
        //rigid.isKinematic = true;
        //rigid.useGravity = false;
        //rigid.velocity = Vector3.zero;
        //rigid.angularVelocity = Vector3.zero;

        itemPrice = itemData.GetRandomPrice();

    }
    public void ShowPickupUI()
    {
        //image.gameObject.SetActive(true);
    }

    public virtual void UseItem() { }


    [ClientRpc]
    public void SetRendererActive(bool isActive)
    {
        renderers.ForEach(rend => rend.enabled = isActive);
    }

    
}
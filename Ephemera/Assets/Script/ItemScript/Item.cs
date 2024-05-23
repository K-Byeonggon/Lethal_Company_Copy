using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Mirror;
public class Item : NetworkBehaviour, IUIVisible, IItemUsable, IItemObtainable
{
    [SerializeField] public ItemData itemData;
    [SyncVar] private int itemPrice;
    public int ItemPrice => itemPrice;
    public Sprite itemSprite => itemData.image;
    public bool IsBothHandGrab => itemData.isBothHand;

    [SerializeField] Collider itemCollider;
    [SerializeField] Rigidbody rigid;

    public override void OnStartServer()
    {
        itemPrice = itemData.GetRandomPrice();
    }
    public void PickUp(Transform pickTransform)
    {
        itemCollider.enabled = false;
        rigid.isKinematic = true;
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
    public void PickDown(Transform pickTransform)
    {
        itemCollider.enabled = true;
        rigid.isKinematic = false;
        rigid.useGravity = true;
        transform.position = pickTransform.position + pickTransform.forward;
        rigid.AddForce(pickTransform.forward * 5.0f, ForceMode.Impulse);
    }

    public void ShowPickupUI()
    {
        //image.gameObject.SetActive(true);
    }

    public void UIvisible()
    {
        //Image image = UIManager.Instance.GetUI<Image>("UI¿Ã∏ß");
        //image.gameObject.SetActive(true);
    }

    public virtual void UseItem() { }


    #region Command Function
    [Command]
    public void CmdChangePosRot(Transform parent)
    {
        OnClientChangePosRot(parent);
    }
    #endregion
    #region ClientRpc Function
    [ClientRpc]
    public void OnClientChangePosRot(Transform parent)
    {
        transform.position = parent.position;
        transform.rotation = parent.rotation;
    }
    #endregion

}
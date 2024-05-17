using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class PlayerEx : MonoBehaviour
{
    //private Item item;
    public float linesize = 1.0f;
    public Image image;
    [SerializeField] public Vector3 moveVector;
    [SerializeField] public Transform pickedItem;
    public Vector3 direction {  get; private set; }
    //private Item item;
    [SerializeField]
    private Inventory inventory;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.ChangeItemSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.ChangeItemSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.ChangeItemSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.ChangeItemSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("detatch");
            inventory.RemovetoInventory();
        }
        Debug.DrawRay(transform.position, transform.forward * linesize, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, linesize))
        {
            Debug.Log(hit.collider.gameObject.name);
            IUIVisible iUIVisible = hit.transform.GetComponent<IUIVisible>();
            if (iUIVisible != null)
            {
                iUIVisible.UIvisible();
            }
            var obtainableItem = hit.transform.GetComponent<Item>();
            if (obtainableItem != null)
            {
                if (inventory.GetCurrentItem() == null&&Input.GetKeyDown(KeyCode.E))
                {
                    //obtainableItem.PickUp(this);
                    inventory.AddtoInventory(obtainableItem.gameObject);
                    Debug.Log("obtainable");
                }
            }
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
    //public void GetItem(Item item)
    //{
    //    this.item = item; 
    //}
    //public void RemoveItem()
    //{
    //    this.item = null;
    //}
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x,0f,input.y);
    }
    void ImageActive()
    {
        image.gameObject.SetActive(true);
    }

}

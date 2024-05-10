using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerEx : MonoBehaviour
{
    //private Item item;
    public float linesize = 16.0f;
    public Image image;
    [SerializeField] public Vector3 moveVector;
    [SerializeField] public Transform pickedItem;
    // Update is called once per frame
    void Update()
    {
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
            var obtainableItem = hit.transform.GetComponent<IItemObtainable>();
            if (obtainableItem != null)
            {
                obtainableItem.PickUp();
                Debug.Log("obtainable");                
                hit.transform.SetParent(transform);
                hit.transform.localPosition = pickedItem.localPosition;
            }
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
    
    void ImageActive()
    {
        image.gameObject.SetActive(true);
    }

}

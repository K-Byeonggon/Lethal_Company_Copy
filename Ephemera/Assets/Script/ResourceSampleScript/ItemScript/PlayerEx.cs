using UnityEditor.Experimental.GraphView;
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
    public Vector3 direction {  get; private set; }
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
            var obtainableItem = hit.transform.GetComponent<Item>();
            if (obtainableItem != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    obtainableItem.PickUp(this);

                    Debug.Log("obtainable");
                }
                else if (obtainableItem != null && Input.GetKeyDown(KeyCode.E))
                {
                    obtainableItem.PickDown(this);
                }
            }
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
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

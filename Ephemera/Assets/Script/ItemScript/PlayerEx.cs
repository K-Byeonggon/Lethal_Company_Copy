using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerEx : MonoBehaviour
{
    public float linesize = 10.0f;
    public Image image;
    [SerializeField] public Vector3 moveVector;
    public Vector3 direction { get; private set; }
    [SerializeField] public Camera camera;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    public int playerHp = 100;
    private int currentHp;
    [SerializeField] public Transform weaponHanger;
    [SerializeField] public GameObject prefab;  
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
        if(playerHp <= 0)
        {
            this.gameObject.SetActive(false);
        }
        

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
        Debug.DrawRay(ray.origin, ray.direction * linesize, Color.yellow);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, linesize))
        {
            //Debug.Log(hit.collider.gameObject.name);
            IUIVisible iUIVisible = hit.transform.GetComponent<IUIVisible>();
            if (iUIVisible != null)
            {
                iUIVisible.UIvisible();
            }
            else
            {
                image.gameObject.SetActive(false);
            }
            var obtainableItem = hit.transform.GetComponent<Item>();
            if (obtainableItem != null)
            {
                if (inventory.GetCurrentItem() == null && Input.GetKeyDown(KeyCode.E))
                {
                    inventory.AddtoInventory(obtainableItem.gameObject);
                    Debug.Log("Item obtained: " + obtainableItem.name);
                }
            }
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Mine"))
        {
            Debug.Log("isDie");
            playerHp = 0;
            Vector3 playerPosition = this.transform.position;  // ���� �÷��̾��� ��ġ�� ����
            Quaternion playerRotation = this.transform.rotation;  // ���� �÷��̾��� ȸ���� ����

            Instantiate(prefab, playerPosition, playerRotation);  // �÷��̾� ��ġ�� ragdoll ������ ����
            prefab.SetActive(true);
            Destroy(this.gameObject);  // ���� �÷��̾� ������Ʈ ����
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
        
    //}
   
}

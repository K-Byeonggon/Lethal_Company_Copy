using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : NetworkBehaviour
{
    #region Field
    private Vector2 _input;
    private Vector3 _direction;
    private float mouseX;
    private float mouseY;
    private float gravity = -9.81f;
    private float _velocity;
    private float currentVelocity;

    private float speed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float jumpforce;
    [SerializeField] private float cameraSpeed;

    [SerializeField] private Transform vCam;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerRaycast playerRaycast;
    [SerializeField] private PlayerHealth playerHealth;
    private bool IsGround() => characterController.isGrounded;
    public bool IsWalking() => _direction != Vector3.zero;
    #endregion

    #region Initialize Function
    public void SetActivateLocalPlayer(bool isActive)
    {
        if (isLocalPlayer)
        {
            Debug.Log("SetActivateLocalPlayer : "+ isActive);
            SetActivateLocalController(isActive);
            this.enabled = isActive;
        }
    }
    public void SetActivateLocalController(bool isActive)
    {
        if (isLocalPlayer)
        {
            characterController.enabled = isActive;
            GetComponent<PlayerInput>().enabled = isActive;
            playerRaycast.enabled = isActive;
        }
    }
    public void SetCameraSpeed(float value)
    {
        cameraSpeed = value;
    }
    #endregion
    #region MonoBehaviour Function
    private void Update()
    {
        if(playerHealth.dead == false)
        {
            ApplyGravity();
            ApplyMovement();
            SetAnimator();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion
    #region NetworkBehaviour Function
    public override void OnStartLocalPlayer()
    {
        if (isServer)
        {
            GameManager.Instance.RegisterPlayer(connectionToClient);
        }
        else
        {
            CmdRegisterPlayer();
        }
    }
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            GameManager.Instance.localPlayerController = this;
            CameraReference.Instance.RegistLocalPlayerVirtualCamera(vCam.gameObject);
            speed = normalSpeed;
        }
        else
            CameraReference.Instance.RegistPlayerVirtualCamera(netId, vCam.gameObject);
    }
    public override void OnStopClient()
    {
        if (isLocalPlayer)
        {
            GameManager.Instance.localPlayerController = null;
            CameraReference.Instance.RegistLocalPlayerVirtualCamera(vCam.gameObject);
        }
        else
            CameraReference.Instance.RegistPlayerVirtualCamera(netId, vCam.gameObject);
    }
    

    #endregion
    #region Update Function
    void ApplyGravity()
    {
        if (IsGround() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }
    void ApplyMovement()
    {
        if(characterController.enabled == true)
            characterController.Move(transform.TransformDirection(_direction) * speed * Time.deltaTime);
    }
    public void SetAnimator()
    {
        animator.SetFloat("Xspeed", _direction.x * speed);
        animator.SetFloat("Zspeed", _direction.z * speed);
    }
    #endregion
    #region InputAction Function
    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector3>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>();
        mouseX += vector2.x * cameraSpeed * Time.deltaTime;
        mouseY -= vector2.y * cameraSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        this.transform.eulerAngles = new Vector3(0, mouseX, 0);
        vCam.localEulerAngles = new Vector3(mouseY, 0, 0);
    }
    public void OnUseItem(InputAction.CallbackContext context)
    {
        if(playerHealth.dead == true)
        {
            CameraReference.Instance.SetActiveNextOtherPlayerVirtualCamera();
        }
        else
        {
            inventory.UseItem();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGround()) return;
        _velocity += jumpforce;
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            speed = runSpeed;
        else if (context.performed) { }
        else if (context.canceled)
            speed = normalSpeed;
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (playerRaycast.HitObject == null)
                return;
            //��ȣ�ۿ� ������Ʈ�� ���
            if (playerRaycast.HitObject.TryGetComponent<Item>(out Item item))
            {
                inventory.AddItem(playerRaycast.HitObject);
                Debug.Log("Item obtained: " + playerRaycast.HitObject.name);
            }
            //���� ���
            else if (playerRaycast.HitObject.TryGetComponent<LinkDoor>(out LinkDoor door))
            {
                CmdTeleport(door.GetTeleportionPosition());
            }
            //������ ���
            else if (playerRaycast.HitObject.TryGetComponent<SellItem>(out SellItem sellItem))
            {
                if (inventory.GetCurrentItemComponent != null)
                {
                    int price = inventory.GetCurrentItemComponent.ItemPrice;
                    sellItem.OnInteractive(price);
                    inventory.RemoveItem();
                }
            }
            //�� ���� ��ȣ�ۿ� ��ǰ�� ���
            else if (playerRaycast.HitObject.TryGetComponent<IInteractive>(out IInteractive InteractiveObject))
            {
                InteractiveObject.OnInteractive();
            }
        }
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {

    }
    public void OnChangeItem(InputAction.CallbackContext context)
    {
        switch(context.control.name)
        {
            case "1":
                inventory.ChangeItemSlot(0);
                break;
            case "2":
                inventory.ChangeItemSlot(1);
                break;
            case "3":
                inventory.ChangeItemSlot(2);
                break;
            case "4":
                inventory.ChangeItemSlot(3);
                break;
        }
    }
    public void OnDetachItem(InputAction.CallbackContext context)
    {
        inventory.ThrowItem();
    }
    #endregion
    #region Network Function
    [Server]
    public void OnServerTeleport(Vector3 position)
    {
        OnClientTeleport(position);
    }
    [Command]
    private void CmdRegisterPlayer()
    {
        GameManager.Instance.RegisterPlayer(connectionToClient);
    }
    [Command(requiresAuthority = false)]
    public void CmdTeleport(Vector3 position)
    {
        OnClientTeleport(position);
    }
    #endregion
    #region Network ClientRpc Function
    [ClientRpc]
    public void OnClientTeleport(Vector3 position)
    {
        transform.position = position;
    }
    #endregion

    public void PlayerDie()
    {
        //CameraReference.Instance
        SetActivateLocalPlayer(false);
    }
    public void PlayerRespawn()
    {
        //CameraReference.Instance.
        //SetActivateLocalPlayer(true);
        playerHealth.Revive();

        if (GameManager.Instance.shipController == null)
            GameManager.Instance.shipController = FindObjectOfType<ShipController>();
        int order = PlayerReference.Instance.GetPlayerOrder(PlayerReference.Instance.localPlayer.netId);
        Transform point = GameManager.Instance.shipController.spawnPoint.GetChild(order);
        CmdTeleport(point.position);
    }

    public void EnbleGameplayControls()
    {

    }
}

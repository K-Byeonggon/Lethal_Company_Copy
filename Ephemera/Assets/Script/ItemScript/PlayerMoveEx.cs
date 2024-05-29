using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveEx : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController character;
    private Vector3 _direction;
    private bool isRun;
    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    [SerializeField] private float smoothTime = 0.05f;
    private float currentVelocity;
    [SerializeField] private float speed;
    [SerializeField] private float runspeed;
    [SerializeField] private float jumpforce;
    [SerializeField] private float cameraSpeed;
    private float mouseX;
    private float mouseY;

    [SerializeField]
    private Transform vCam;

    private Animator animator;
    PlayerStat playerStat;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
        SetAnimator();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (isRun)
        {
            playerStat.UpdateStamina();
            if (playerStat.stamina <= 0)
            {
                StopRunning();
            }
        }
        else
        {
            playerStat.RefillStamina();
        }
    }

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

    void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0)
            return;
        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    void ApplyMovement()
    {
        character.Move(transform.TransformDirection(_direction) * speed * Time.deltaTime);
    }

    public void PlayerMovement(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector3>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (playerStat.stamina <= 0)
            return;

        if (context.started)
        {
            speed = runspeed;
            isRun = true;
        }
        else if (context.canceled)
        {
            StopRunning();
        }
    }

    public void StopRunning()
    {
        speed = 2.0f;
        isRun = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGround()) return;

        _velocity += jumpforce;
    }

    public void SetAnimator()
    {
        animator.SetFloat("Xspeed", _direction.x * speed);
        animator.SetFloat("Zspeed", _direction.z * speed);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>();
        mouseX += vector2.x * cameraSpeed * Time.deltaTime;
        mouseY -= vector2.y * cameraSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0);
        vCam.localEulerAngles = new Vector3(mouseY, 0, 0);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        Vector2 moveVector = context.ReadValue<Vector2>();
    }

    public void EnableGameplayControls()
    {

    }

    private bool IsGround() => character.isGrounded;
    public bool IsWalking()
    {
        return _direction != Vector3.zero;
    }
}

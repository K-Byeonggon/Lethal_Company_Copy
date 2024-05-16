using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : NetworkBehaviour
{
    private Vector2 _input;
    private CharacterController character;
    private Vector3 _direction;

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
    [SerializeField]
    private GameObject vCam;

    private Animator animator;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            GetComponent<PlayerInput>().enabled = false;
            character.enabled = false;
            this.enabled = false;
            vCam.SetActive(false);
        }
    }

    private void Update()
    {
        animator.SetBool("IsWalking", false);
        if (Input.anyKey)
        {
            animator.SetBool("IsWalking", true);
        }
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
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
        //sda
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
        //_input = context.ReadValue<Vector2>();
        //_direction = new Vector3(_input.x , 0.0f, _input.y);
        //_direction = transform.forward * context.ReadValue<Vector2>();
        Vector2 moveVector = context.ReadValue<Vector2>();
        _direction = new Vector3(moveVector.x, 0, moveVector.y);

    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            speed = 50.0f;
            animator.SetBool("IsRun", true);
        }
        else if(context.performed)
        {

        }
        else if (context.canceled)
        {
            speed = 30.0f;
            animator.SetBool("IsRun", false);
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 vec = context.ReadValue<Vector2>();
        mouseX += vec.x * cameraSpeed;
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGround()) return;

        _velocity += jumpforce;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        Vector2 moveVector = context.ReadValue<Vector2>();
    }

    public void EnbleGameplayControls()
    {

    }


    private bool IsGround() => character.isGrounded;
}

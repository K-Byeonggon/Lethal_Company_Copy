using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float runSpeed = 1.0f;
    [SerializeField] private float gravity = 40.0f;
    [SerializeField] private float jumpForce = 10.0f;

    float maxDistance = 0.1f;

    private Vector2 mouseDelta;
    private CharacterController controller;
    private Animator animator;

    public Transform cameraAngle;
    public float mouseSensitivity = 10f;
    private float xRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsGrounded())
            moveVector.y -= gravity * Time.deltaTime;
        controller.Move(transform.TransformDirection(moveVector) * moveSpeed * runSpeed * Time.deltaTime);
        SetAnimator();

        Debug.DrawRay(transform.position, -transform.up * maxDistance, Color.red);
    }

    void LateUpdate()
    {
        animator.SetFloat("IsSpeed", mouseDelta.magnitude);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector3>();

    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started) runSpeed = 2.0f;
        else if (context.performed) { }
        else if (context.canceled) runSpeed = 1.0f;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
        cameraAngle.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsGrounded()) moveVector.y = jumpForce;
    }

    private void SetAnimator()
    {
        animator.SetFloat("XSpeed", moveVector.x * runSpeed);
        animator.SetFloat("ZSpeed", moveVector.z * runSpeed);
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, maxDistance);
        if (hit.collider != null) { /*Debug.Log(hit.collider.name);*/ return true; }
        else { /*Debug.Log("¾ÈºÎ‹HÈû");*/ return false; }
    }
}

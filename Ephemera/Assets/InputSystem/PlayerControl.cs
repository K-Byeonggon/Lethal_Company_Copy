using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Player Control")]
    public float speed = 5.0f; 
    public float runspeed = 8.0f;
    public float crouch = 3.0f;  
    public float jumpheight = 0.8f;
    public float gravitymultiplier = 1.0f;
    public float crouchCollheigth = 1.5f; 

    Rigidbody rigid;

    public Vector2 inputVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector2 nextvec = inputVec.normalized * speed * Time.deltaTime;
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        Debug.Log("ds");
        
    }

    void OnJump()
    {

    }


}

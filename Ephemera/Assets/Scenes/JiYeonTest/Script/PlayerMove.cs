using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public Vector2 _inputmove;
    public Vector3 _direction;
    public float speed;

    private CharacterController _char;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _char = GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _inputmove = context.ReadValue<Vector2>();
        _direction = new Vector3(_inputmove.x, 0.0f, _inputmove.y);

    }
}
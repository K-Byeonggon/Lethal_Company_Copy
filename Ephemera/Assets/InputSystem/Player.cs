using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Player player;
    private InputAction move;

    private Rigidbody rigd;
    [SerializeField]
    private float moveforec = 1.0f;
    [SerializeField]
    private float jumpforec = 5.0f;
    [SerializeField]
    private float maxspeed = 5.0f;
    private Vector3 forecDir = Vector3.zero;
    [SerializeField]
    private Camera playcamera;

    private void Awake()
    {
        rigd = this.GetComponent<Rigidbody>();
    }




}

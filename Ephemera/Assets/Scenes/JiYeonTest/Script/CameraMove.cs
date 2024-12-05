using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public static CameraMove instance;

    [SerializeField] public float mouseSpeed;
    private float mouseX = 0f;
    private float mouseY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        instance = this;

        mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        mouseY = Mathf.Clamp(mouseY, -50f, 90f);
        mouseX = Mathf.Clamp(mouseX, -50f, 90f);

        this.transform.localEulerAngles = new Vector3(-mouseY, mouseX, 0);
    }
}

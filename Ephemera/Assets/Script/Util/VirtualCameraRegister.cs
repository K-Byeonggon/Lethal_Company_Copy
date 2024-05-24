using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraRegister : MonoBehaviour
{
    [SerializeField]
    VirtualCameraType type;
    private void Awake()
    {
        CameraReference.Instance.RegistVirtualCamera(type, gameObject);
    }
    private void OnDestroy()
    {
        CameraReference.Instance.DeregistVirtualCamera(type);
    }
}

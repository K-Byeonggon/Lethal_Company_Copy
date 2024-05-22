using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVirtualCameraRegister : NetworkBehaviour
{
    private void Awake()
    {
        CameraReference.Instance.RegistPlayerVirtualCamera(netId, gameObject);
        if (isLocalPlayer)
            CameraReference.Instance.RegistLocalPlayerCamera(gameObject);
    }
    private void OnDestroy()
    {
        CameraReference.Instance.DeregistPlayerVirtualCamera(netId);
    }
}

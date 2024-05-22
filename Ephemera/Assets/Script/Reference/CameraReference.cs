using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraReference : SingleTon<CameraReference>
{
    GameObject loaclPlayerCamera;
    Dictionary<uint, GameObject> playerVirtualCameraDic = new Dictionary<uint, GameObject>();
    Dictionary<VirtualCameraType, GameObject> virtualCameraDic = new Dictionary<VirtualCameraType, GameObject>();


    public void RegistLocalPlayerCamera(GameObject cameraObject)
    {
        loaclPlayerCamera = cameraObject;
    }
    //vCam���
    public void RegistPlayerVirtualCamera(uint netId, GameObject cameraObject)
    {
        playerVirtualCameraDic.TryAdd(netId, cameraObject);
    }
    //vCam����
    public void DeregistPlayerVirtualCamera(uint netId)
    {
        if (playerVirtualCameraDic.ContainsKey(netId))
        {
            playerVirtualCameraDic.Remove(netId);
        }
    }
    //vCam���
    public void RegistVirtualCamera(VirtualCameraType type, GameObject cameraObject)
    {
        virtualCameraDic.TryAdd(type, cameraObject);
    }
    //vCam����
    public void DeregistVirtualCamera(VirtualCameraType type)
    {
        if(virtualCameraDic.ContainsKey(type))
        {
            virtualCameraDic.Remove(type);
        }
    }


    //playerVCamȰ��
    public void SetActivePlayerVirtualCamera(uint netId)
    {
        if (playerVirtualCameraDic.TryGetValue(netId, out GameObject value))
        {
            foreach (var playerVirtualCamera in playerVirtualCameraDic.Values)
            {
                if (playerVirtualCamera == value)
                    playerVirtualCamera.SetActive(true);
                else
                    playerVirtualCamera.SetActive(false);
            }

            foreach (var virtualCamera in virtualCameraDic.Values)
            {
                virtualCamera.SetActive(false);
            }
        }
    }
    //vCamȰ��
    public void SetActiveVirtualCamera(VirtualCameraType type)
    {
        if (virtualCameraDic.TryGetValue(type, out GameObject value))
        {
            foreach (var virtualCamera in virtualCameraDic.Values)
            {
                if (virtualCamera == value)
                    virtualCamera.SetActive(true);
                else
                    virtualCamera.SetActive(false);
            }
        }
        else
        {
            foreach (var virtualCamera in virtualCameraDic.Values)
            {
                virtualCamera.SetActive(false);
            }
        }
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CameraReference : SingleTon<CameraReference>
{
    GameObject loaclPlayerCamera;
    Dictionary<uint, GameObject> playerVirtualCameraDic = new Dictionary<uint, GameObject>();
    Dictionary<VirtualCameraType, GameObject> objectVirtualCameraDic = new Dictionary<VirtualCameraType, GameObject>();


    #region LocalPlayer
    public void RegistLocalPlayerVirtualCamera(GameObject cameraObject)
    {
        loaclPlayerCamera = cameraObject;
    }
    //vCam����
    public void DeregistLocalPlayerVirtualCamera()
    {
        loaclPlayerCamera = null;
    }
    #endregion
    #region Other Player
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
    #endregion

    #region Object VCam
    //vCam���
    public void RegistVirtualCamera(VirtualCameraType type, GameObject cameraObject)
    {
        objectVirtualCameraDic.TryAdd(type, cameraObject);
    }
    //vCam����
    public void DeregistVirtualCamera(VirtualCameraType type)
    {
        if(objectVirtualCameraDic.ContainsKey(type))
        {
            objectVirtualCameraDic.Remove(type);
        }
    }
    #endregion

    #region Active Vcam
    // loacl Player Camera�� Ȱ���ϰ� ������ ��Ȱ��
    public void SetActiveLocalPlayerVirtualCamera()
    {
        loaclPlayerCamera.SetActive(true);
        foreach (var item in playerVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        foreach (var item in objectVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
    }
    // other player VCamȰ��
    public void SetActivePlayerVirtualCamera(uint netId)
    {
        if (playerVirtualCameraDic.ContainsKey(netId) == false)
            return;

        foreach (var playerVirtualCamera in playerVirtualCameraDic)
        {
            if (playerVirtualCamera.Key == netId)
                playerVirtualCamera.Value.SetActive(true);
            else
                playerVirtualCamera.Value.SetActive(false);
        }
        foreach (var virtualCamera in objectVirtualCameraDic.Values)
        {
            virtualCamera.SetActive(false);
        }
        loaclPlayerCamera.SetActive(false);
    }
    //vCamȰ��
    public void SetActiveVirtualCamera(VirtualCameraType type)
    {
        if (objectVirtualCameraDic.ContainsKey(type) == false)
            return;

        GameObject camera = objectVirtualCameraDic[type];
        foreach (var virtualCamera in objectVirtualCameraDic.Values)
        {
            if (virtualCamera == camera)
                virtualCamera.SetActive(true);
            else
                virtualCamera.SetActive(false);
        }
        loaclPlayerCamera.SetActive(false);
    }
    #endregion
}

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
    //vCam해제
    public void DeregistLocalPlayerVirtualCamera()
    {
        loaclPlayerCamera = null;
    }
    #endregion
    #region Other Player
    //vCam등록
    public void RegistPlayerVirtualCamera(uint netId, GameObject cameraObject)
    {
        playerVirtualCameraDic.TryAdd(netId, cameraObject);
    }
    //vCam해제
    public void DeregistPlayerVirtualCamera(uint netId)
    {
        if (playerVirtualCameraDic.ContainsKey(netId))
        {
            playerVirtualCameraDic.Remove(netId);
        }
    }
    #endregion

    #region Object VCam
    //vCam등록
    public void RegistVirtualCamera(VirtualCameraType type, GameObject cameraObject)
    {
        objectVirtualCameraDic.TryAdd(type, cameraObject);
    }
    //vCam해제
    public void DeregistVirtualCamera(VirtualCameraType type)
    {
        if(objectVirtualCameraDic.ContainsKey(type))
        {
            objectVirtualCameraDic.Remove(type);
        }
    }
    #endregion

    #region Active Vcam
    // loacl Player Camera를 활성하고 나머지 비활성
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
    // other player 첫번째 VCam활성
    public void SetActiveFirstOtherPlayerVirtualCamera()
    {
        if (playerVirtualCameraDic.Count == 0)
            return;
        playerVirtualCameraDic.Values.First().SetActive(false);
        loaclPlayerCamera.SetActive(false);
    }
    // other player 다음 VCam활성
    public void SetActiveNextOtherPlayerVirtualCamera()
    {
        if (playerVirtualCameraDic.Count == 0)
            return;
        GameObject activeCam = null;
        List<GameObject> values = new List<GameObject>(playerVirtualCameraDic.Values);
        foreach (var item in values)
        {
            if (item.activeInHierarchy == true)
                activeCam = item;
        }
        if (activeCam == null)
            return;
        //활성화된 카메라의 번호
        int currentIndex = values.IndexOf(activeCam);
        if (currentIndex == values.Count - 1)
            currentIndex = 0;
        else
            currentIndex += 1;


        loaclPlayerCamera.SetActive(false);
        foreach (var item in playerVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        foreach (var item in objectVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        values[currentIndex].SetActive(true);
    }
    
    // other player VCam활성
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
    //vCam활성
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

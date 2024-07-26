using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CameraReference : SingleTon<CameraReference>
{
    GameObject _localPlayerCamera;
    Dictionary<uint, GameObject> _playerVirtualCameraDic = new Dictionary<uint, GameObject>();
    Dictionary<VirtualCameraType, GameObject> _objectVirtualCameraDic = new Dictionary<VirtualCameraType, GameObject>();

    #region LocalPlayer
    public void RegistLocalPlayerVirtualCamera(GameObject cameraObject)
    {
        _localPlayerCamera = cameraObject;
    }
    //vCam����
    public void DeregistLocalPlayerVirtualCamera()
    {
        _localPlayerCamera = null;
    }
    #endregion
    #region Other Player
    //vCam���
    public void RegistPlayerVirtualCamera(uint netId, GameObject cameraObject)
    {
        _playerVirtualCameraDic.TryAdd(netId, cameraObject);
    }
    //vCam����
    public void DeregistPlayerVirtualCamera(uint netId)
    {
        if (_playerVirtualCameraDic.ContainsKey(netId))
        {
            _playerVirtualCameraDic.Remove(netId);
        }
    }
    #endregion
    #region Object VCam
    //vCam���
    public void RegistVirtualCamera(VirtualCameraType type, GameObject cameraObject)
    {
        _objectVirtualCameraDic.TryAdd(type, cameraObject);
    }
    //vCam����
    public void DeregistVirtualCamera(VirtualCameraType type)
    {
        if(_objectVirtualCameraDic.ContainsKey(type))
        {
            _objectVirtualCameraDic.Remove(type);
        }
    }
    #endregion
    #region Active Vcam
    // loacl Player Camera 자신의 카메라 활성화
    public void SetActiveLocalPlayerVirtualCamera()
    {
        Debug.Log("Local Player Virtual Camera");
        _localPlayerCamera.SetActive(true);
        foreach (var item in _playerVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        foreach (var item in _objectVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
    }
    // other player 첫번째 카메라 활성화
    public void SetActiveFirstOtherPlayerVirtualCamera()
    {
        Debug.Log("Other Player Virtual Camera");
        if (_playerVirtualCameraDic.Count == 0)
            return;
        _playerVirtualCameraDic.Values.First().SetActive(true);
        _localPlayerCamera.SetActive(false);
    }
    // other player 다음 카메라 활성화
    public void SetActiveNextOtherPlayerVirtualCamera()
    {
        if (_playerVirtualCameraDic.Count == 0)
            return;
        GameObject activeCam = null;
        List<GameObject> values = new List<GameObject>(_playerVirtualCameraDic.Values);
        foreach (var item in values)
        {
            if (item.activeInHierarchy == true)
                activeCam = item;
        }
        if (activeCam == null)
            return;
        
        int currentIndex = values.IndexOf(activeCam);
        if (currentIndex == values.Count - 1)
            currentIndex = 0;
        else
            currentIndex += 1;


        _localPlayerCamera.SetActive(false);
        foreach (var item in _playerVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        foreach (var item in _objectVirtualCameraDic)
        {
            item.Value.SetActive(false);
        }
        values[currentIndex].SetActive(true);
    }
    // other player 플레이어 카메라 활성화
    public void SetActivePlayerVirtualCamera(uint netId)
    {
        if (_playerVirtualCameraDic.ContainsKey(netId) == false)
            return;

        foreach (var playerVirtualCamera in _playerVirtualCameraDic)
        {
            if (playerVirtualCamera.Key == netId)
                playerVirtualCamera.Value.SetActive(true);
            else
                playerVirtualCamera.Value.SetActive(false);
        }
        foreach (var virtualCamera in _objectVirtualCameraDic.Values)
        {
            virtualCamera.SetActive(false);
        }
        _localPlayerCamera.SetActive(false);
    }
    
    //vCam 오브젝트 카메라 활성화
    public void SetActiveVirtualCamera(VirtualCameraType type)
    {
        if (_objectVirtualCameraDic.ContainsKey(type) == false)
        {
            Debug.Log("Don't have a virtual camera");
            return;
        }

        foreach (var virtualCamera in _objectVirtualCameraDic.Values)
        {
            virtualCamera.SetActive(false);
        }
        foreach (var virtualCamera in _playerVirtualCameraDic.Values)
        {
            virtualCamera.SetActive(false);
        }
        
        _localPlayerCamera.SetActive(false);
        
        GameObject camera = _objectVirtualCameraDic[type];
        camera.SetActive(true);
    }
    #endregion
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraReference : SingleTon<CameraReference>
{
    Dictionary<VirtualCameraType, GameObject> virtualCameraDic = new Dictionary<VirtualCameraType, GameObject>();

    //vCam등록
    public void RegistVirtualCamera(VirtualCameraType type, GameObject cameraObject)
    {
        virtualCameraDic.TryAdd(type, cameraObject);
    }
    //vCam해제
    public void DeregistVirtualCamera(VirtualCameraType type)
    {
        if(virtualCameraDic.ContainsKey(type))
        {
            virtualCameraDic.Remove(type);
        }
    }

    //vCam활성
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

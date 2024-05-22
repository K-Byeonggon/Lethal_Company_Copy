using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraReference : SingleTon<CameraReference>
{
    Dictionary<VirtualCameraType, GameObject> virtualCameraDic = new Dictionary<VirtualCameraType, GameObject>();

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

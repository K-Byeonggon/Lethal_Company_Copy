using UnityEngine;

public class FindMic : MonoBehaviour
{
    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Available device: " + device);
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found.");
        }
    }
}

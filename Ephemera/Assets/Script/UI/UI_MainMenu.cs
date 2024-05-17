using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    public void StartHost()
    {
        GameRoomNetworkManager.Instance.StartHost();
    }
    public void StartClient()
    {
        GameRoomNetworkManager.Instance.StartClient();
    }
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

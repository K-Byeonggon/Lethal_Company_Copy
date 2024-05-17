using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    [SerializeField]
    List<PlayerPanel> playerPanels;
    public const int textureWidth = 350;
    public const int textureHeight = 600;
    public const int depth = 24; // ±Ì¿Ã πˆ∆€ ∫Ò∆Æ ±Ì¿Ã

    [SerializeField]
    private GameObject clientPrefab;

    public void Awake()
    {
        Init();
    }
    public void Init()
    {
        foreach (var item in playerPanels)
        {
            item.stateTextUI.text = "Waiting";
            item.state = false;
        }
    }

    public void AddClient(int clientIndex)
    {
        GameObject client = Instantiate(clientPrefab);
        Camera camera = client.GetComponentInChildren<Camera>();
        RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, depth);
        renderTexture.Create();
        camera.targetTexture = renderTexture;
        playerPanels[clientIndex].rawImage.texture = renderTexture;
    }

    public void ChangeClientState(int clientIndex)
    {
        switch (playerPanels[clientIndex].state)
        {
            case true:
                playerPanels[clientIndex].state = false;
                playerPanels[clientIndex].stateTextUI.text = "Waiting";
                break;
            case false:
                playerPanels[clientIndex].state = true;
                playerPanels[clientIndex].stateTextUI.text = "Ready";
                break;
        }
    }
}
[Serializable]
public class PlayerPanel
{
    public RawImage rawImage;
    public TextMeshProUGUI stateTextUI;
    public bool state = false;
}


using UnityEngine;
using Mirror;

public class VoiceChatClient : NetworkBehaviour
{
    public string microphoneDevice; // 마이크 장치의 이름
    public float voiceInterval = 0.1f; // 음성을 보낼 간격

    private float voiceTimer = 0f;
    private AudioClip microphoneInput;

    void Start()
    {
        microphoneInput = Microphone.Start(microphoneDevice, true, 1, AudioSettings.outputSampleRate);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        voiceTimer += Time.deltaTime;
        if (voiceTimer >= voiceInterval)
        {
            SendVoiceData();
            voiceTimer = 0f;
        }
    }

    void SendVoiceData()
    {
        int microphoneLength = Microphone.GetPosition(microphoneDevice);
        float[] data = new float[microphoneLength];
        microphoneInput.GetData(data, 0);

        CmdSendVoiceData(data);
    }

    [Command]
    void CmdSendVoiceData(float[] data)
    {
        RpcReceiveVoiceData(data);
    }

    [ClientRpc]
    void RpcReceiveVoiceData(float[] data)
    {
        AudioClip receivedAudio = AudioClip.Create("Voice", data.Length, 1, AudioSettings.outputSampleRate, false);
        receivedAudio.SetData(data, 0);
        AudioSource.PlayClipAtPoint(receivedAudio, transform.position);
    }
}

using UnityEngine;
using Mirror;

public class VoiceChatClient : NetworkBehaviour
{
    public string microphoneDevice; // 마이크 장치의 이름
    public float voiceInterval = 0.1f; // 음성을 보낼 간격

    private float voiceTimer = 0f;
    private AudioClip microphoneInput;
    public NetworkBehaviour[] NetworkBehaviours { get; private set; } = new NetworkBehaviour[] { };
    void Start()
    {
        microphoneInput = Microphone.Start(Microphone.devices[0].ToString(), true, 1, AudioSettings.outputSampleRate);
        Debug.Log(Microphone.devices[0].ToString());
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

        // 서버에 음성 데이터를 보냅니다.
        CmdSendVoiceData(data);
    }

    [Command]
    void CmdSendVoiceData(float[] data)
    {
        // 서버에서 모든 클라이언트에게 음성 데이터를 전달합니다.
        RpcReceiveVoiceData(data);
    }

    [ClientRpc]
    void RpcReceiveVoiceData(float[] data)
    {
        // 받은 음성 데이터를 재생합니다.
        AudioClip receivedAudio = AudioClip.Create("Voice", data.Length, 1, AudioSettings.outputSampleRate, false);
        receivedAudio.SetData(data, 0);
        AudioSource.PlayClipAtPoint(receivedAudio, transform.position);
    }
}

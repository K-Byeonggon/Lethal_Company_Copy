using UnityEngine;
using Mirror;

public class VoiceChatServer : NetworkBehaviour
{
    [Command]
    void CmdSendVoiceData(float[] data)
    {
        // 보낸 클라이언트를 제외하고 모든 클라이언트에게 음성 데이터 전송
        RpcReceiveVoiceData(data);
    }

    [ClientRpc]
    void RpcReceiveVoiceData(float[] data)
    {
        // 서버에서 받은 음성 데이터 재생
        AudioClip receivedAudio = AudioClip.Create("Voice", data.Length, 1, AudioSettings.outputSampleRate, false);
        receivedAudio.SetData(data, 0);
        AudioSource.PlayClipAtPoint(receivedAudio, transform.position);
    }
}

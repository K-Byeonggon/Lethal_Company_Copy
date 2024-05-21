using UnityEngine;
using Mirror;
using System;

public class VoiceChatServer : NetworkBehaviour
{
    [Command]
    void CmdSendVoiceDataToServer(float[] data)
    {
        RpcReceiveVoiceData(data);
    }
    [ClientRpc]
    void RpcReceiveVoiceData(float[] data)
    {
        using (NetworkWriterPooled writer = new NetworkWriterPooled())
        {
            SerializeFloatArray(writer, data);
            SendDataToAllClients(writer.ToArray());
        }
    }

    void SerializeFloatArray(NetworkWriter writer, float[] array)
    {
        writer.WriteArray(array);
    }

    void SendDataToAllClients(byte[] data)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn != null && conn != connectionToClient)
            {
                // VoiceDataMessage 생성
                VoiceDataMessage message = new VoiceDataMessage();
                message.data = data;
                // 메시지를 보냄 (기본적으로 Reliable 채널 사용)
               // conn.Send<VoiceDataMessage>(message, 0);
            }
        }
    }

    // Send 메서드 추가
    public void Send<T>(T message, int channelId = Channels.Reliable) where T : struct, NetworkMessage
    {
        NetworkServer.SendToAll(message, channelId);
    }
}

public class VoiceDataMessage : NetworkMessage
{
    public byte[] data;
}

using UnityEngine;
using Mirror;

public class ServerPlayerMove : NetworkBehaviour
{
    Vector3 _moveDirection = Vector3.zero;
    [Client]
    private void Update()
    {
        if(isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 playerMovement = new Vector3(h * 0.25f, v * 0.25f, 0);

            _moveDirection = playerMovement;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                CmdMove();
            }
        }
    }

    //Command, 클라이언트에서 서버로 명령을 전송하는 어트리뷰트
    [Command]
    private void CmdMove()
    {
        //로직 유효성 검사
        //TODO

        RpcMove();
    }
    //ClientRpc, 서버에서 클라이언트들에세 
    [ClientRpc]
    private void RpcMove()
    {
        //transform.Translate(_moveDirection);
        transform.Translate(Vector3.up);
    }
}

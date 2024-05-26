using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepController : NetworkBehaviour
{
    public LayerMask groundLayer;
    public LayerMask metalLayer;
    public AudioSource footstepSource;
    public AudioClip groundFootstep;
    public AudioClip metalFootstep;
    [SerializeField]
    PlayerController playerController;
    private float rayDistance = 1.0f; // 레이캐스트 거리

    // 애니메이션 이벤트에 연결될 함수
    [Command]
    public void PlayFootstepSound()
    {
        if (playerController.IsWalking())
        {
            SurfaceType surfaceType = DetectSurface();
            PlaySound(surfaceType);
        }
    }

    private SurfaceType DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayer))
        {
            return SurfaceType.Ground;
        }
        else if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, metalLayer))
        {
            return SurfaceType.Metal;
        }
        return SurfaceType.Ground;
    }

    [ClientRpc]
    private void PlaySound(SurfaceType surfaceType)
    {
        switch (surfaceType)
        {
            case SurfaceType.Ground:
                //Debug.Log("Ground");
                footstepSource.clip = groundFootstep;
                break;
            case SurfaceType.Metal:
                //Debug.Log("Metal");
                footstepSource.clip = metalFootstep;
                break;
        }

        footstepSource.Play();
    }
}

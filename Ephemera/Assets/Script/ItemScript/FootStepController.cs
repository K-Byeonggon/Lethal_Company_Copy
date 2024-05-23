using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepController : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask metalLayer;
    public AudioSource footstepSource;
    public AudioClip groundFootstep;
    public AudioClip metalFootstep;
    [SerializeField]
    PlayerMoveEx playerMove;
    private float rayDistance = 1.0f; // 레이캐스트 거리

    // 애니메이션 이벤트에 연결될 함수
    public void PlayFootstepSound()
    {
        if (playerMove.IsWalking())
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

    private void PlaySound(SurfaceType surfaceType)
    {
        switch (surfaceType)
        {
            case SurfaceType.Ground:
                Debug.Log("Ground");
                footstepSource.clip = groundFootstep;
                break;
            case SurfaceType.Metal:
                Debug.Log("Metal");
                footstepSource.clip = metalFootstep;
                break;
                // 필요한 경우 다른 케이스를 추가합니다
        }

        footstepSource.Play();
    }
}

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
    private float rayDistance = 1.0f; // ����ĳ��Ʈ �Ÿ�

    // �ִϸ��̼� �̺�Ʈ�� ����� �Լ�
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
                // �ʿ��� ��� �ٸ� ���̽��� �߰��մϴ�
        }

        footstepSource.Play();
    }
}

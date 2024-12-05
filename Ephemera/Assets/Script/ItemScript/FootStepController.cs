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
    private float rayDistance = 1.0f; // ����ĳ��Ʈ �Ÿ�

    // �ִϸ��̼� �̺�Ʈ�� ����� �Լ�
    [Command(requiresAuthority = false)]
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

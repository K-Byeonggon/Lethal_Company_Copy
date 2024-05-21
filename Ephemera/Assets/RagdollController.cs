using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    GameObject Spine;
    [SerializeField]
    GameObject Thigh_L;
    [SerializeField]
    GameObject Thigh_R;
    [SerializeField]
    GameObject Mesh;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.J))
        {
            Thigh_L.transform.SetParent(Spine.transform, true);
            Thigh_R.transform.SetParent(Spine.transform, true);
            Mesh.transform.SetParent(Spine.transform, true);

            Spine.transform.SetParent(null, true);

            Destroy(gameObject);

            animator.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public float linesize = 10.0f;
    [SerializeField] private Transform vCam;
    public GameObject hitObject = null;
    public GameObject HitObject => hitObject;
    private void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(vCam.position, vCam.forward);//camera.ScreenPointToRay(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
        Debug.DrawRay(ray.origin, ray.direction * linesize, Color.yellow);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, linesize))
        {
            //Debug.Log(hit.collider.gameObject.name);
            IUIVisible iUIVisible = hit.transform.GetComponent<IUIVisible>();
            if (iUIVisible != null)
            {
                UIController.Instance.ui_Game.UIInteraction(true);
                hitObject = hit.transform.gameObject;
            }
            else
            {
                UIController.Instance.ui_Game.UIInteraction(false);
                hitObject = null;
            }
        }
        else
        {
            UIController.Instance.ui_Game.UIInteraction(false);
            hitObject = null;
        }
    }
    public void Interaction()
    {
        
    }
}

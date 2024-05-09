using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PlayerEx : MonoBehaviour
{
    
    public float linesize = 16.0f;
    public Image image;
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * linesize, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,linesize))
        {
            Debug.Log(hit.collider.gameObject.name);
            ImageActive();
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
    void ImageActive()
    {
        image.gameObject.SetActive(true);
    }
    
}

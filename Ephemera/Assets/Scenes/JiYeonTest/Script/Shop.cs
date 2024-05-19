using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log(collision.gameObject.name);
            UI.instance.Interaction.gameObject.SetActive(true);
        }
    }
    void OnCollisionExit(Collision collision)
    {
        UI.instance.Interaction.gameObject.SetActive(false);
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellTrigger : NetworkBehaviour
{
    public Button Sellbutton;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SellEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SellEixt();
        }
    }

    void SellEnter()
    {
        Sellbutton.gameObject.SetActive(true);
    }

    void SellEixt()
    {
        Sellbutton.gameObject.SetActive(false);
    }

}

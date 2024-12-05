using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetItemCoroutine());
    }

    private IEnumerator GetItemCoroutine()
    {
        yield return new WaitForSeconds(5f);
        item.transform.parent = transform;
    }
}

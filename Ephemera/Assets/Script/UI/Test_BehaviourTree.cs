using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Test_BehaviourTree : SingleTon<Test_BehaviourTree>
{
    [SerializeField] public TextMeshProUGUI nodeStatus;

    // Start is called before the first frame update
    void Start()
    {
        nodeStatus.text = "hello";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

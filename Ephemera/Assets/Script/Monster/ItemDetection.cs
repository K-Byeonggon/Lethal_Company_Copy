using UnityEngine;

public class ItemDetection : MonoBehaviour
{
    YipeeAI yipee;
    float nestIgnoreRadius = 0.5f;
    private void Start()
    {
        yipee = transform.parent.GetComponent<YipeeAI>();
    }
    // 아이템 탐지 범위에 들어왔을 때 호출되는 함수
    
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("뭔가 닿음");
       

        if (other.gameObject.tag == "ObtainableItem")
        {
            Debug.Log("아이템 닿았엉");
            float distanceToNest = Vector3.Distance(other.transform.position, yipee.nest.position);
            //둥지 근처에 있는 아이템은 탐지하지 않음.
            if (distanceToNest > nestIgnoreRadius)
            {   //이미 플레이어의 손이나 자신의 손에 들려있는 아이템은 탐지하지 않음.
                if (other.transform.parent == null)
                {
                    yipee.itemFind = true;
                    yipee.detectedItem = other.gameObject;
                }
            }
        }
    }
}
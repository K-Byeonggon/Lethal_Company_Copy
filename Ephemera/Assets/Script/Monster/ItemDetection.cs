using UnityEngine;

public class ItemDetection : MonoBehaviour
{
    YipeeAI yipee;
    float nestIgnoreRadius = 1f;
    private void Start()
    {
        yipee = transform.parent.GetComponent<YipeeAI>();
    }
    // ������ Ž�� ������ ������ �� ȣ��Ǵ� �Լ�
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ObtainableItem")
        {
            float distanceToNest = Vector3.Distance(other.transform.position, yipee.nest.position);
            //���� ��ó�� �ִ� �������� Ž������ ����.
            if (distanceToNest > nestIgnoreRadius)
            {   //�̹� �÷��̾��� ���̳� �ڽ��� �տ� ����ִ� �������� Ž������ ����.
                if (other.transform.parent == null)
                {
                    yipee.itemFind = true;
                    yipee.detectedItem = other.gameObject;
                }
            }
        }

        if(other.gameObject.tag == "Player")
        {
            yipee.bewareOf = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            yipee.bewareOf = null;
        }
    }
}
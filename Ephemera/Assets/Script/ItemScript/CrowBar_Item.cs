using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CrowBar_Item : Item, IItemUsable
{
    private Quaternion originalRotation;
    private bool isUsingItem = false;

    private void Start()
    {
        originalRotation = transform.rotation;
        Collider collider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    public override void UseItem()
    {
        if (!isUsingItem)
        {
            StartCoroutine(SwingAndReset());
        }
    }

    private IEnumerator SwingAndReset()
    {
        isUsingItem = true;
        Quaternion intermediateRotation = Quaternion.Euler(45, 0, 0); // Intermediate swing position
        Quaternion targetRotation = Quaternion.Euler(90, 0, 0); // Final swing position
        float forwardDuration = 0.3f; // Duration for forward swing
        float backwardDuration = 0.5f; // Duration for backward swing

        // Rotate to intermediate position
        yield return RotateTo(intermediateRotation, forwardDuration / 2);

        // Rotate to target position
        yield return RotateTo(targetRotation, forwardDuration / 2);

        Debug.Log("collideron");
        
        

        yield return new WaitForSeconds(0.5f); // Wait for the attack duration

        // Rotate back to intermediate position
        yield return RotateTo(intermediateRotation, backwardDuration / 2);

        // Rotate back to the original rotation
        yield return RotateTo(originalRotation, backwardDuration / 2);

        isUsingItem = false;
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy")){
            //Enemy.hp -= 10;
        }
    }
}

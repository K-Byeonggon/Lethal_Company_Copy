using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CrowBar_Item : Item, IItemUsable
{
    private Quaternion originalRotation;
    private bool isUsingItem = false;
    private bool isCharging = false;
    private float chargeTime = 2.0f;
    Collider weaponCollider;
    private void Start()
    {
        originalRotation = transform.localRotation;
        weaponCollider = GetComponent<Collider>();
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
        weaponCollider.enabled = true;
        isUsingItem = true;
        Quaternion intermediateRotation = Quaternion.Euler(45, 0, 0); 
        Quaternion targetRotation = Quaternion.Euler(75, 0, 0); 
        float forwardDuration = 0.3f; 
        float backwardDuration = 0.5f; 

        
        yield return RotateTo(intermediateRotation, forwardDuration );

        
        yield return RotateTo(targetRotation, forwardDuration );


        yield return new WaitForSeconds(0.5f); 

        yield return RotateTo(intermediateRotation, backwardDuration );
        

        yield return RotateTo(originalRotation, backwardDuration );
        isUsingItem = false;
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = transform.localRotation;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weaponCollider.enabled = false;
        transform.localRotation = targetRotation;
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monster")){
            LivingEntity livingEntity = collision.gameObject.GetComponent<LivingEntity>();
            Debug.Log("ũ�ο�ٿ� ������ :" + collision.gameObject.name);
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.damager = gameObject;
            damageMessage.damage = 10;

            livingEntity.ApplyDamage(damageMessage);
            //if(collision.gameObject.CompareTag("Enemy")){
            //    //Enemy.hp -= 10;
            //}
        }
    }
}

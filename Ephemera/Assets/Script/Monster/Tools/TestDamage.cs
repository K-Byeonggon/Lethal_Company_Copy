using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamage : MonoBehaviour
{
    [SerializeField] GameObject Yipee;
    DamageMessage damageMessage;
    // Start is called before the first frame update
    void Start()
    {
        damageMessage.damage = 4f;
        damageMessage.damager = gameObject;

        StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Yipee.GetComponent<LivingEntity>().ApplyDamage(damageMessage);
        }
    }

}

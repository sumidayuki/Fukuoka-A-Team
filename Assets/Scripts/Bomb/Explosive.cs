using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Enemy"))
        {
            IDamageable damageable = other.gameObject.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(1);
            }
        }
    }
}

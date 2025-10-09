using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlokenBlock : MonoBehaviour, IDamageable
{
    public void Damage(float amount)
    {
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public void Damage(float amount)
    {
        StageManager.Instance.OnEnemyDestroyed(this.gameObject);
        gameObject.SetActive(false);
    }
}

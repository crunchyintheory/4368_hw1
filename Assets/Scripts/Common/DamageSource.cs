using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] public int Team;

    [SerializeField] public float Damage = 1;
    [SerializeField] public bool Piercing = false;

    private DestroyEffects _destroyEffects;

    private void Start()
    {
        this._destroyEffects = GetComponent<DestroyEffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Damageable d = other.gameObject.GetComponent<Damageable>();
        if (d)
        {
            if (d.Team == this.Team) return;
            d.ModifyHealth(-this.Damage, this);
        }

        if (!this.Piercing)
        {
            this._destroyEffects?.Destroy(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public int Team { get; protected set; }

    [SerializeField] public float Damage = 1;
    [SerializeField] public bool Piercing = false;

    private DestroyEffects _destroyEffects;

    private void Start()
    {
        this._destroyEffects = GetComponent<DestroyEffects>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Damageable d = other.gameObject.GetComponent<Damageable>();
        if (d)
        {
            d.ModifyHealth(-this.Damage, this);
        }

        if (!this.Piercing)
        {
            this._destroyEffects?.Destroy(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
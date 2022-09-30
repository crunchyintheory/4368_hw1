using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageSource : MonoBehaviour, ITeamable
{
    [SerializeField] private int _team;

    public int Team
    {
        get => this._team;
        set => this._team = value;
    }

    [SerializeField] public int Damage = 1;
    [SerializeField] public bool Piercing = false;
    [SerializeField] public bool BlocksOtherProjectiles = false;
    [SerializeField] public bool FriendlyFire = false;
    [SerializeField] public float WarmupTime = 0;
    [SerializeField] private EffectBundle _destroyEffects;

    [SerializeField] private LayerMask _collisionMask;
    
    private float _spawnTime;

    private void Awake()
    {
        this._spawnTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this._collisionMask.value);
        if ((this._collisionMask.value & 1<<other.gameObject.layer) == 0)
        {
            return;
        }

        if (Time.time - this._spawnTime < this.WarmupTime)
            return;
        
        ITeamable teamable = other.gameObject.GetComponent<ITeamable>();
        if (teamable != null && teamable.Team == this.Team && !this.FriendlyFire)
            return;
        
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(this.Damage, this);

        DamageSource damageSource = other.gameObject.GetComponent<DamageSource>();
        if (damageSource && !damageSource.BlocksOtherProjectiles)
            return;

        if (!this.Piercing)
        {
            this._destroyEffects?.Play(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
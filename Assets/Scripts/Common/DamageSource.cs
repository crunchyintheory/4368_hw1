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
    [SerializeField, Min(1)] public int ProjectileLayer = 0;
    [SerializeField] public int ProjectileBlockMask = 0;
    [SerializeField] public bool FriendlyFire = false;
    [SerializeField] public float WarmupTime = 0;
    [SerializeField] private EffectBundle _destroyEffects;

    [SerializeField] private LayerMask _collisionMask;

    public LayerMask CollisionMask
    {
        get => this._collisionMask;
        set => this._collisionMask = value;
    }
    
    private float _spawnTime;

    private void Awake()
    {
        this._spawnTime = Time.time;
    }

    public void OnTriggerEnter(Collider other)
    {
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
        if (damageSource && (damageSource.ProjectileLayer & this.ProjectileBlockMask) == 0)
            return;

        if (!this.Piercing)
        {
            this._destroyEffects?.Play(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
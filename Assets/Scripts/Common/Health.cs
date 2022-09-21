using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, ITeamable
{
    [SerializeField] private int _initialHealth;
    [SerializeField] private int _team;
    [SerializeField] private int _damageCap = 0;

    [SerializeField] private bool _destroyOnDeath = false;

    [SerializeField] private EffectBundle _damageEffects;

    public int CurrentHealth { get; protected set; }

    public int Team
    {
        get => this._team;
        protected set => this._team = value;
    }

    private DamageSource _lastTouched;

    public delegate void DamagedEventHandler(object sender, DamageSource source);
    public event DamagedEventHandler OnDamaged;

    public delegate void KilledEventHandler(object sender, DamageSource lastTouched);
    public event KilledEventHandler OnKill;

    private void Start()
    {
        this.CurrentHealth = this._initialHealth;
    }

    public virtual void TakeDamage(int damage, DamageSource source)
    {
        if (this._damageCap > 0)
            damage = Mathf.Min(damage, this._damageCap);
        this.CurrentHealth -= damage;
        this.OnDamaged?.Invoke(this, source);
        this._lastTouched = source;
        this._damageEffects?.Play(this.transform.position);
        if (this.CurrentHealth <= 0)
            Kill();
    }

    public void Kill()
    {
        this.OnKill?.Invoke(this, this._lastTouched);
        if(!this._destroyOnDeath) Destroy(this.gameObject);
    }
}
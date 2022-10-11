using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour, IKillable, ITeamable
{
    [SerializeField] protected int _initialHealth;
    [SerializeField] protected int _team;
    [SerializeField] protected int _damageCap = 0;
    [SerializeField] protected float _damageDebounce = 0.5f;

    [SerializeField] protected bool _destroyOnDeath = false;

    [SerializeField] protected EffectBundle _damageEffects;

    protected float _lastDamagedTime = -999f;

    public virtual int CurrentHealth { get; protected set; }
    public int MaxHealth => this._initialHealth;

    public bool IsImmune = false;

    public int Team
    {
        get => this._team;
        protected set => this._team = value;
    }

    protected DamageSource _lastTouched;

    public virtual event IDamageable.DamagedEventHandler OnDamaged;

    public virtual event IKillable.KilledEventHandler OnKilled;

    private void Start()
    {
        this.CurrentHealth = this._initialHealth;
    }

    public virtual void TakeDamage(int damage, DamageSource source)
    {
        if (this.IsImmune) return;
        
        if (this._damageCap > 0)
            damage = Mathf.Min(damage, this._damageCap);

        if (this._damageDebounce > 0 && Time.time - this._lastDamagedTime < this._damageDebounce)
            return;

        this._lastDamagedTime = Time.time;
        this.CurrentHealth = Mathf.Max(0, this.CurrentHealth - damage);
        this.OnDamaged?.Invoke(this, source);
        this._lastTouched = source;
        this._damageEffects?.Play(this.transform.position);
        if (this.CurrentHealth <= 0)
            Kill();
    }

    public virtual void Kill()
    {
        this.OnKilled?.Invoke(this, this._lastTouched);
        if(!this._destroyOnDeath) Destroy(this.gameObject);
    }
}
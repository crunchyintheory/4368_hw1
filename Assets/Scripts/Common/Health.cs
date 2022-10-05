using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour, IKillable, ITeamable
{
    [SerializeField] private int _initialHealth;
    [SerializeField] private int _team;
    [SerializeField] private int _damageCap = 0;
    [SerializeField] private float _damageDebounce = 0.5f;

    [SerializeField] private bool _destroyOnDeath = false;

    [SerializeField] private EffectBundle _damageEffects;

    private float _lastDamagedTime = -999f;

    public int CurrentHealth { get; protected set; }
    public int MaxHealth => this._initialHealth;

    public int Team
    {
        get => this._team;
        protected set => this._team = value;
    }

    private DamageSource _lastTouched;

    public event IDamageable.DamagedEventHandler OnDamaged;

    public event IKillable.KilledEventHandler OnKilled;

    private void Start()
    {
        this.CurrentHealth = this._initialHealth;
    }

    public virtual void TakeDamage(int damage, DamageSource source)
    {
        if (this._damageCap > 0)
            damage = Mathf.Min(damage, this._damageCap);

        if (Time.time - this._lastDamagedTime < this._damageDebounce)
            return;

        this._lastDamagedTime = Time.time;
        this.CurrentHealth = Mathf.Max(0, this.CurrentHealth - damage);
        this.OnDamaged?.Invoke(this, source);
        this._lastTouched = source;
        this._damageEffects?.Play(this.transform.position);
        if (this.CurrentHealth <= 0)
            Kill();
    }

    public void Kill()
    {
        this.OnKilled?.Invoke(this, this._lastTouched);
        if(!this._destroyOnDeath) Destroy(this.gameObject);
    }
}
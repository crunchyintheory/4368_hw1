using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SharedHealth : Health
{
    [SerializeField] private string _poolName = "";

    private static readonly Dictionary<string, List<SharedHealth>> SharedObjects = new();
    private static readonly Dictionary<string, int> Pools = new();

    public override event IDamageable.DamagedEventHandler OnDamaged;
    public override event IKillable.KilledEventHandler OnKilled;

    private int _singletonCurrentHealth;
    
    public override int CurrentHealth
    {
        get
        {
            if (this._poolName == "") return this._singletonCurrentHealth;
            Pools.TryGetValue(this._poolName, out int health);
            return health;
        }
        protected set
        {
            if (this._poolName == "") this._singletonCurrentHealth = value;
            Pools[this._poolName] = value;
        }
    }

    private void OnEnable()
    {
        if (this._poolName == "") return;
        if (!SharedObjects.TryGetValue(this._poolName, out List<SharedHealth> list))
        {
            list = new List<SharedHealth>();
            SharedObjects.Add(this._poolName, list);
        }
        list.Add(this);
        if (!Pools.ContainsKey(this._poolName))
        {
            Pools.Add(this._poolName, this._initialHealth);
        }
    }

    private void OnDisable()
    {
        if (SharedObjects.TryGetValue(this._poolName, out List<SharedHealth> list))
        {
            //list.Remove(this);
        }
    }
    
    private void Start()
    {
        this.CurrentHealth = this._initialHealth;
        this._singletonCurrentHealth = this._initialHealth;
    }
    
    public override void TakeDamage(int damage, DamageSource source)
    {
        if (this._damageCap > 0)
            damage = Mathf.Min(damage, this._damageCap);

        if (this._damageDebounce > 0 && Time.time - this._lastDamagedTime < this._damageDebounce)
            return;

        this._lastDamagedTime = Time.time;
        this.CurrentHealth = Mathf.Max(0, this.CurrentHealth - damage);
        if (SharedObjects.TryGetValue(this._poolName, out List<SharedHealth> allPoolMembers))
        {
            allPoolMembers.ForEach(x => x.OnDamaged?.Invoke(this, source));
        }
        else
        {
            this.OnDamaged?.Invoke(this, source);
        }
        this._lastTouched = source;
        this._damageEffects?.Play(this.transform.position);
        if (this.CurrentHealth <= 0)
            Kill();
    }
    
    public override void Kill()
    {
        if (this._poolName == "" || !SharedObjects.TryGetValue(this._poolName, out List<SharedHealth> objects))
        {
            this.OnKilled?.Invoke(this, this._lastTouched);
            if(!this._destroyOnDeath) Destroy(this.gameObject);            
        }
        else
        {
            objects.ForEach(x =>
            {
                x.OnKilled?.Invoke(x, this._lastTouched);
                if(x._destroyOnDeath) Destroy(x.gameObject);  
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float _initialHealth;
    
    public float Health { get; protected set; }
    public int Team { get; protected set; }

    private DamageSource _lastTouched;

    public delegate void DamagedEventHandler(object sender, DamageSource source);
    public event DamagedEventHandler OnDamaged;

    public delegate void KilledEventHandler(object sender, DamageSource lastTouched);
    public event KilledEventHandler OnKill;

    private void Start()
    {
        this.Health = this._initialHealth;
    }

    public void ModifyHealth(float delta, DamageSource source)
    {
        this.Health += delta;
        this.OnDamaged?.Invoke(this, source);
        this._lastTouched = source;
        if (this.Health <= 0)
            Kill();
    }

    public void Kill()
    {
        this.OnKill?.Invoke(this, this._lastTouched);
        Destroy(this.gameObject);
    }
}
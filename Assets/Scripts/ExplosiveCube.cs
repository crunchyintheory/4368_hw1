using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveCube : MonoBehaviour, IDamageable, ITeamable
{
    private static List<ExplosiveCube> Cubes = new();

    [SerializeField] private GameObject _visual;
    [SerializeField] private EffectBundle _enableEffects;
    [SerializeField] private float _enableWaitTime;
    [SerializeField] private EffectBundle _disableEffects;
    [SerializeField] private float _disableWaitTime;
    [SerializeField] private EffectBundle _destroyEffects;

    [SerializeField] private bool PosZ;
    [SerializeField] private int _team;

    private bool _enabled = true;

    private Boss _target;

    public int Team { get; }

    private void OnEnable()
    {
        Cubes.Add(this);
    }

    private void OnDisable()
    {
        Cubes.Remove(this);
    }

    private void Start()
    {
         SetBossTarget();
    }

    public event IDamageable.DamagedEventHandler OnDamaged;

    public void TakeDamage(int amount, DamageSource source)
    {
        if (!this._enabled) return;
        this.OnDamaged?.Invoke(this, source);
        this._enabled = false;
        this._destroyEffects?.Play(this.transform.position);
        this._visual.SetActive(false);
        this._target.InterruptMove(this.PosZ);
    }

    public static void EnableAll()
    {
        Cubes.ForEach(x => x.Enable());
        ResetBossTarget();
    }

    public static void DisableAll()
    {
        Cubes.ForEach(x => x.Disable());
    }

    public static void ResetBossTarget()
    {
        Cubes.ForEach(x => x.SetBossTarget());
    }

    private void SetBossTarget()
    {
        float closestDistance = float.MaxValue;
        Boss closest = Boss.Bosses.First();
        Boss.Bosses.ForEach(boss =>
        {
            float distance = (boss.transform.position - this.transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closest = boss;
                closestDistance = distance;
            }
        });

        this._target = closest;
    }

    private void Disable()
    {
        if (!this._enabled) return;
        this._enabled = false;
        StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        this._disableEffects?.Play(this.transform.position);
        yield return new WaitForSeconds(this._disableWaitTime);
        this._visual.SetActive(false);
    }

    private void Enable()
    {
        if (this._enabled) return;
        this._enabled = true;
        StartCoroutine(EnableCoroutine());
    }

    private IEnumerator EnableCoroutine()
    {
        this._enableEffects?.Play(this.transform.position);
        yield return new WaitForSeconds(this._enableWaitTime);
        this._visual.SetActive(true);
    }
}

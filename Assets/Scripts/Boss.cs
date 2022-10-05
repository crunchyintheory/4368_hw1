using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase, RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    private Coroutine _attackCoroutine;

    [Header("Movement")]
    [SerializeField] private float _moveTime = 1.5f;
    [SerializeField] private Transform[] _positions;
    [SerializeField] private int _position = 1;
    [SerializeField] private float _baseHeight = -4.93f;
    [SerializeField] private float _hideHeight = -14f;
    [SerializeField] private float _hideTime = 5f;

    [Header("Attacks")]
    [SerializeField] private float _laserAttackChance = 0.1f;
    private static float _currentLaserAttackChanceMultiplier = 1.5f;

    [SerializeField] private Transform _projectileSpawnLocation;

    [Header("Attacks")]
    [SerializeField] private DamageSource _rocket;
    [SerializeField] private DamageSource _energyBall;
    [SerializeField] private LayerMask _energyLayerMask;
    [SerializeField] private Beam _beamTemplate;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem _energyCharge;
    [SerializeField] private ParticleSystem _missileCharge;
    [SerializeField] private EffectBundle _explosion;

    private int _phase = 1;

    private int _index = 0;
    private static int _nextIndex = 0;

    private static List<Boss> _bosses = new();
    public static List<Boss> Bosses
    {
        get => _bosses;
    }

    private Animator _animator;

    private static int _laserAttackerThisTurn = -1;

    private Health _health;

    private Beam _beam;
    
    // Solves a race condition
    private static void DecideLaserAttack()
    {
        if (_laserAttackerThisTurn != -1) return;
        IEnumerable<Boss> validTargets = _bosses.Where(x => x._position == 2);
        foreach (Boss boss in validTargets)
        {
            if (Random.value >= 1 - boss._laserAttackChance * _currentLaserAttackChanceMultiplier)
            {
                _laserAttackerThisTurn = boss._index;
                break;
            }
        }

        if (_laserAttackerThisTurn == -1)
        {
            _currentLaserAttackChanceMultiplier *= 1f;
        }
    }

    private static void ResetLaserAttack()
    {
        _currentLaserAttackChanceMultiplier = 1;
        _laserAttackerThisTurn = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        this._animator = GetComponent<Animator>();
        this._animator.enabled = false;
        this._index = _nextIndex++;
        _bosses.Add(this);
        this.transform.position = this._positions[this._position].position;
        BossStart();
    }

    void OnEnable()
    {
        this._health = GetComponentInChildren<Health>();
        this._health.OnKilled += DestroyEffects;
    }

    void OnDisable()
    {
        this._health.OnKilled -= DestroyEffects;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BossStart()
    {
        this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
    }

    private void SpawnProjectile(int max, float maxAngle, int projNum = 0)
    {
        float angle;
        if (max % 2 == 1)
            angle = (maxAngle / max) * (-2 + projNum);
        else
            angle = (maxAngle / max) * (-2 + projNum + 0.5f);

        DamageSource projectile = Instantiate(this._energyBall, this._projectileSpawnLocation.position, Quaternion.Euler(0, angle, 0) * this._projectileSpawnLocation.rotation);
        projectile.CollisionMask = this._energyLayerMask;
        projectile.Team = 1;

        projectile.GetComponent<EffectBundle>().Volume = 0.1f;

        if (++projNum < max)
        {
            SpawnProjectile(max, maxAngle, projNum);
        }
    }

    private void DestroyEffects(object sender, DamageSource killer)
    {
        StopAllCoroutines();
        StartCoroutine(OnDestroyCoroutine());
    }

    private IEnumerator OnDestroyCoroutine()
    {
        this._health.gameObject.SetActive(false);
        Vector3 sunkenPos = this.transform.position;
        sunkenPos.y = this._hideHeight;

        Vector3 oldPos = this.transform.position;
        
        for (float i = 0; i < 5; i += Time.fixedDeltaTime)
        {
            if (Random.value >= 0.3)
            {
                Vector3 placement = this.transform.position + Random.insideUnitSphere * 5;
                this._explosion.Play(placement);
            }

            this.transform.position = Vector3.Lerp(oldPos, sunkenPos, i / 5);

            yield return new WaitForFixedUpdate();
        }
        
        Destroy(this._beam);
        
        this.gameObject.SetActive(false);
    }

    private void DoLaserAttack()
    {
        ExplosiveCube.DisableAll();
        this._attackCoroutine = StartCoroutine(LaserAttackCoroutine());
        ResetLaserAttack();
    }

    private IEnumerator LaserAttackCoroutine()
    {
        Instantiate(this._energyCharge, this._projectileSpawnLocation);
        this._beam = Instantiate(this._beamTemplate, this._projectileSpawnLocation.position, this._projectileSpawnLocation.rotation,
            this._projectileSpawnLocation);
        this._beam.StartLocation = this._projectileSpawnLocation;
        this._beam.transform.localPosition += new Vector3(0, 1.152f, 0);

        Boss other = _bosses.Find(x => x._index != this._index);
        if (other.isActiveAndEnabled)
        {
            other.StopAllCoroutines();
            other.StartCoroutine(other.LaserAttackHideCoroutine());
        }
        yield return new WaitForSeconds(this._hideTime);

        Transform[] positions = this._positions!.Clone() as Transform[]; // Shallow copy doesn't return type correctly, so force it
        this._positions = other._positions!.Clone() as Transform[];
        other._positions = positions;

        this._animator.enabled = true;
        this._animator.SetTrigger(this.transform.position.x < 0 ? "MoveForward" : "MoveBackward");
        yield return new WaitForSeconds(10f);
        this._animator.enabled = false;
        Destroy(this._beam.gameObject);
        
        yield return new WaitForSeconds(this._hideTime);

        yield return new WaitForSeconds(2f);
        this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
        ExplosiveCube.EnableAll();
    }

    private IEnumerator LaserAttackHideCoroutine()
    {
        Vector3 hidePosition = this.transform.position;
        hidePosition.y = this._hideHeight;
        for (float i = 0; i < this._hideTime; i += Time.deltaTime)
        {
            this.transform.position = Vector3.Lerp(this._positions[this._position].position, hidePosition, i/this._hideTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(10f);
        
        // Update hidePosition as it has now changed (see LaserAttackCoroutine)
        hidePosition = this._positions[this._position].position;
        hidePosition.y = this._hideHeight;
        this.transform.rotation = Quaternion.Euler(0, 180 - this.transform.rotation.eulerAngles.y, 0);
        
        for (float i = 0; i < this._hideTime; i += Time.deltaTime)
        {
            this.transform.position = Vector3.Lerp(hidePosition, this._positions[this._position].position, i/this._hideTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2f);
        this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
    }

    private IEnumerator MainAttackCoroutine()
    {
        DecideLaserAttack();
        for (int i = 0; i < 3; i++)
        {
            Instantiate(this._missileCharge, this._projectileSpawnLocation);
            yield return new WaitForSeconds(1.5f);
            // do missile attack;
            if (this._rocket)
            {
                Instantiate(this._rocket, this._projectileSpawnLocation.position, this._projectileSpawnLocation.rotation);
                Instantiate(this._rocket, this._projectileSpawnLocation.position + Vector3.forward, this._projectileSpawnLocation.rotation);
                Instantiate(this._rocket, this._projectileSpawnLocation.position + Vector3.back, this._projectileSpawnLocation.rotation);
            }
            yield return new WaitForSeconds(0.1f);
        }
        // do a projectile attack;
        Instantiate(this._energyCharge, this._projectileSpawnLocation);
        yield return new WaitForSeconds(1.5f);
        SpawnProjectile(5, 160);
        yield return new WaitForSeconds(0.25f);
        SpawnProjectile(4, 120);
        yield return new WaitForSeconds(0.25f);
        SpawnProjectile(5, 160);
        yield return new WaitForSeconds(0.25f);
        
        yield return new WaitForSeconds(3f);

        if (this._positions.Length > 1 && this._phase >= 1)
        {
            if (_laserAttackerThisTurn == this._index) // Value is artificially high for testing purposes.
            {
                // Use the laser attack
                DoLaserAttack();
            }
            else if (_laserAttackerThisTurn == -1)
            {
                //move
                int old = this._position;
                int pos;
                do
                {
                    pos = Mathf.RoundToInt(Random.Range(0, this._positions.Length));
                } while (pos == old);
                int difference = Mathf.Abs(this._position - pos);
                this._position = pos;

                for (float i = 0; i < this._moveTime * difference; i += Time.deltaTime)
                {
                    this.transform.position = Vector3.Lerp(this._positions[old].position, this._positions[pos].position, i / (this._moveTime * difference));
                    yield return new WaitForEndOfFrame();
                }

                this.transform.position = this._positions[pos].position;

                yield return new WaitForSeconds(2f);
                this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
            }
        }
    }
}

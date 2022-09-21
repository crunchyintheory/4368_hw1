using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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

    [SerializeField] private Transform _projectileSpawnLocation;

    [Header("Effects")]
    [SerializeField] private ProjectileMovement _rocket;
    [SerializeField] private ProjectileMovement _energyBall;
    [SerializeField] private EffectBundle _explosion;
    
    private int _phase = 1;

    private int _index = 0;
    private static int _nextIndex = 0;

    private static List<Boss> _bosses = new List<Boss>();

    private Animator _animator;

    private static int _laserAttackerThisTurn = -1;

    private Health _health;
    
    // Solves a race condition
    private static void DecideLaserAttack()
    {
        if (_laserAttackerThisTurn != -1) return;
        IEnumerable<Boss> validTargets = _bosses.Where(x => x._position == 2);
        foreach (Boss boss in validTargets)
        {
            if (Random.value >= 1 - boss._laserAttackChance)
            {
                _laserAttackerThisTurn = boss._index;
                break;
            }
        }
    }

    private static void ResetLaserAttack()
    {
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
        this._health = GetComponentInChildren<Health>();
        this._health.OnKill += DestroyEffects;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BossStart()
    {
        this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
    }

    private void SpawnProjectile(int projNum, int max)
    {
        
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
        
        this.gameObject.SetActive(false);
    }

    private void DoLaserAttack()
    {
        Boss other = _bosses.Find(x => x._index != this._index);
        if (other.isActiveAndEnabled)
        {
            other.StopAllCoroutines();
            other.StartCoroutine(other.LaserAttackHideCoroutine());
        }
        this._attackCoroutine = StartCoroutine(LaserAttackCoroutine(other));
        ResetLaserAttack();
    }

    private IEnumerator LaserAttackCoroutine(Boss other)
    {
        yield return new WaitForSeconds(this._hideTime);

        Transform[] positions = this._positions!.Clone() as Transform[]; // Shallow copy doesn't return type correctly, so force it
        this._positions = other._positions!.Clone() as Transform[];
        other._positions = positions;

        this._animator.enabled = true;
        this._animator.SetTrigger(this.transform.position.x < 0 ? "MoveForward" : "MoveBackward");
        yield return new WaitForSeconds(10f);
        this._animator.enabled = false;
        
        yield return new WaitForSeconds(this._hideTime);

        yield return new WaitForSeconds(2f);
        this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
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
            // do missile attack;
            if (this._rocket)
            {
                Instantiate(this._rocket, this._projectileSpawnLocation.position, this._projectileSpawnLocation.rotation);
                Instantiate(this._rocket, this._projectileSpawnLocation.position + Vector3.forward, this._projectileSpawnLocation.rotation);
                Instantiate(this._rocket, this._projectileSpawnLocation.position + Vector3.back, this._projectileSpawnLocation.rotation);
            }
            yield return new WaitForSeconds(1.5f);
        }
        for (int i = 0; i < 3; i++)
        {
            // do a projectile attack;
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(1.5f);

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    private Coroutine _attackCoroutine;

    private int _position = 1;
    [Header("Movement")]
    [SerializeField] private float _moveTime = 1.5f;
    [SerializeField] private Transform[] _positions;
    [SerializeField] private float _baseHeight = -4.93f;
    [SerializeField] private float _hideHeight = -14f;
    [SerializeField] private float _hideTime = 5f;

    [Header("Attacks")]
    [SerializeField] private float _laserTime = 10f;

    [Header("Effects")]
    [SerializeField] private ProjectileMovement _rocket;
    [SerializeField] private ProjectileMovement _energyBall;
    [SerializeField] private ParticleSystem _explosion;
    
    private int _phase = 1;

    private int _index = 0;
    private static int _nextIndex = 0;

    private static List<Boss> _bosses = new List<Boss>();

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        this._animator = GetComponent<Animator>();
        this._animator.enabled = false;
        this._index = _nextIndex++;
        _bosses.Add(this);
        BossStart();
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

    private IEnumerator LaserAttackCoroutine()
    {
        Boss other = _bosses.Find(x => x._index != this._index);
        other.StopAllCoroutines();
        other.transform.position = other._positions[other._position].position;
        other.StartCoroutine(other.LaserAttackHideCoroutine());

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

        yield return new WaitForSeconds(this._laserTime);
        
        // Update hidePosition as it has now changed (see LaserAttackCoroutine)
        hidePosition = this.transform.position;
        hidePosition.y = this._hideHeight;
        
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
        for (int i = 0; i < 3; i++)
        {
            // do missile attack;
            if(this._explosion) Instantiate(this._explosion, this.transform.position + (Vector3.forward * 5), Quaternion.identity);
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
            if (this._position == 2 && Random.value > 0f) // Value is artificially high for testing purposes.
            {
                // Use the laser attack
                this._attackCoroutine = StartCoroutine(LaserAttackCoroutine());
                yield break;
            }
            //move
            int old = this._position;
            int pos;
            do
            {
                pos = Mathf.RoundToInt(Random.Range(0, this._positions.Length));
            } while (pos == old);
            int difference = Mathf.Abs(this._position - pos);
            this._position = pos;

            Debug.Log($"Moving {this.gameObject.name} to position {pos} from {old}");
            Debug.Log(difference);
            for (float i = 0; i < this._moveTime * difference; i += Time.deltaTime)
            {
                Debug.Log($"{Time.time}: iterating {i}");
                this.transform.position = Vector3.Lerp(this._positions[old].position, this._positions[pos].position, i / (this._moveTime * difference));
                yield return new WaitForEndOfFrame();
            }

            this.transform.position = this._positions[pos].position;
            
            Debug.Log($"Finished moving {this.gameObject.name}; restarting loop");

            yield return new WaitForSeconds(2f);
            this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
        }
    }
}

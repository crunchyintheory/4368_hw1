using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Boss : MonoBehaviour
{
    private Coroutine _attackCoroutine;

    private int _position = 1;
    [Header("Movement")]
    [SerializeField] private float _moveTime = 1.5f;
    [SerializeField] private Transform[] _positions;

    [Header("Effects")]
    [SerializeField] private ProjectileMovement _rocket;
    [SerializeField] private ProjectileMovement _energyBall;
    [SerializeField] private ParticleSystem _explosion;
    
    private int _phase = 0;

    // Start is called before the first frame update
    void Start()
    {
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

        if (this._positions.Length <= 1 && this._phase > 1)
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

            for (float i = 0; i < 1; i += Time.deltaTime / (this._moveTime * difference))
            {
                this.transform.position = Vector3.Lerp(this._positions[old].position, this._positions[pos].position, i);
                yield return new WaitForEndOfFrame();
            }
            this._attackCoroutine = StartCoroutine(MainAttackCoroutine());
        }
    }
}

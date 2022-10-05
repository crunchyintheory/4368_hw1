using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _initialVelocity;
    [SerializeField] private float _life = 60;

    public Vector3 Velocity { get; protected set; }

    private Rigidbody _rb;
    private float _spawnTime;

    private void Start()
    {
        this._spawnTime = Time.time;
        this._rb = GetComponent<Rigidbody>();
        this.Velocity = this._initialVelocity;
    }

    private void Update()
    {
        if (Time.time - this._spawnTime > this._life)
        {
            Destroy(this.gameObject);
        }
        this._rb.velocity = this.transform.rotation * this.Velocity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _initialVelocity;

    public Vector3 Velocity { get; protected set; }

    private Rigidbody _rb;

    private void Start()
    {
        this._rb = GetComponent<Rigidbody>();
        this.Velocity = this._initialVelocity;
    }

    private void Update()
    {
        this._rb.velocity = this.transform.rotation * this.Velocity;
    }
}

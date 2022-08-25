using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleBase : MonoBehaviour
{
    protected abstract void Collect(Player player);

    [SerializeField] private float _movementSpeed = 1;
    protected float MovementSpeed => this._movementSpeed;

    [SerializeField] private ParticleSystem _collectParticles;
    [SerializeField] private AudioClip _collectSound;

    private Rigidbody _rb;

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Movement(this._rb);
    }

    protected virtual void Movement(Rigidbody rb)
    {
        // calculate rotation
        Quaternion turnOffset = Quaternion.Euler(0, this._movementSpeed, 0);
        rb.MoveRotation(this._rb.rotation * turnOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Collect(player);
            Disable();
        }
    }

    protected virtual void Disable()
    {
        // spawn particles & sfx because we need to disable object
        Feedback();
            
        this.gameObject.SetActive(false);
    }

    protected virtual void Feedback()
    {
        // particles
        if (this._collectParticles != null)
        {
            this._collectParticles = Instantiate(this._collectParticles, this.transform.position, Quaternion.identity);
        }
        //audio. TODO - consider Obejct (sic) Pooling for performance
        if (this._collectSound != null)
        {
            AudioHelper.PlayClip2D(this._collectSound, 1f);
        }
    }
}

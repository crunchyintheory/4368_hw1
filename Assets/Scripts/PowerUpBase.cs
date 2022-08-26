using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember
// ^ GetComponent<> is the preferred means to obtain components now, and .renderer and .collider are now deprecated properties

[RequireComponent(typeof(Rigidbody))]
public abstract class PowerUpBase : MonoBehaviour
{
    protected abstract void PowerUp(Player player);
    protected abstract void PowerDown(Player player);
    
    // ReSharper disable once IdentifierTypo
    [SerializeField] private float _powerupDuration = 5f;
    [SerializeField] private ParticleSystem _collectParticles;
    [SerializeField] private AudioClip _collectSound;

    [SerializeField] private float _movementSpeed = 1;
    protected float MovementSpeed => this._movementSpeed;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        this._rb = GetComponent<Rigidbody>();
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Disable();
            PowerUp(player);
            yield return new WaitForSeconds(this._powerupDuration);
            PowerDown(player);
        }
    }
    
    private void Disable()
    {
        // spawn particles & sfx because we need to disable object
        Feedback();
        
        // disable visuals and collision
        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null) renderer.enabled = false;
        
        Collider collider = GetComponent<Collider>();
        if(collider != null) collider.enabled = false;
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
    
    private void FixedUpdate()
    {
        Movement(this._rb);
    }

    protected virtual void Movement(Rigidbody rb)
    {
        // calculate rotation
        Quaternion turnOffset = Quaternion.Euler(this._movementSpeed, 0, 0);
        rb.MoveRotation(this._rb.rotation * turnOffset);
    }
}

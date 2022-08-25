using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private ParticleSystem _impactParticles;
    [SerializeField] private AudioClip _impactSound;

    private Rigidbody _rb;

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            PlayerImpact(player);
            ImpactFeedback();
        }
    }

    protected virtual void PlayerImpact(Player player)
    {
        player.DecreaseHealth(this._damageAmount);
    }

    private void ImpactFeedback()
    {
        //particles
        if (this._impactParticles != null)
        {
            this._impactParticles = Instantiate(this._impactParticles, this.transform.position, Quaternion.identity);
        }
        //audio. TODO - consider Object Pooling for performance
        if (this._impactSound != null)
        {
            AudioHelper.PlayClip2D(this._impactSound, 1f);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffects : MonoBehaviour
{
    [SerializeField] public AudioClip DestroySounds;
    [SerializeField] public float Volume;
    [SerializeField] public ParticleSystem DestroyParticles;
    
    public void Destroy(Vector3 position)
    {
        if (this.DestroySounds)
        {
            AudioHelper.PlayClip2D(this.DestroySounds, 1.0f);
        }

        if (this.DestroyParticles)
        {
            Instantiate(this.DestroyParticles, position, Quaternion.identity);
        }
    }
}

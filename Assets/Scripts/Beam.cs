using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] public Transform StartLocation;
    [SerializeField] private LayerMask _rayMask;
    [SerializeField] private LayerMask _damageMask;
    [SerializeField] public float ActivationTime = 1.5f;
    [SerializeField] private ParticleSystem _chargeEffect;

    private DamageSource _damageSource;
    private float _spawnTime;

    private ParticleSystem _ps;
    
    void OnEnable()
    {
        this._spawnTime = Time.time;
        this._damageSource = GetComponent<DamageSource>();
        this._ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float length = 30;
        if (Physics.Raycast(
                origin: this.StartLocation.position,
                direction: this.StartLocation.forward,
                maxDistance: 30,
                layerMask: this._rayMask,
                hitInfo: out RaycastHit hit))
        {
            length = hit.distance;
        }
        float scale = length;
        float posZ = length * 1.244f; // I have NO idea what this number is, but apparently it's what's needed to make the position line up.
        Vector3 localScale = this.transform.localScale;
        Vector3 localPos = this.transform.localPosition;

        localScale.z = scale;
        localPos.z = posZ;
            
        this.transform.localPosition = localPos;
        this._chargeEffect.transform.localScale = localScale;
        this.transform.localScale = localScale;

        Vector3 halfExtents = this.transform.lossyScale / 2;
        halfExtents.y = 10;

        if (Time.time - this._spawnTime >= this.ActivationTime)
        {
            Collider[] colliders = Physics.OverlapBox(
                center: this.transform.position,
                halfExtents: halfExtents,
                orientation: this.transform.rotation,
                layerMask: this._damageMask,
                queryTriggerInteraction: QueryTriggerInteraction.Collide
            );

            // For some reason, collision isn't working on the tank. So I have to do this
            foreach (Collider collider in colliders)
            {
                this._damageSource.OnTriggerEnter(collider);
            }
        }
    }
}

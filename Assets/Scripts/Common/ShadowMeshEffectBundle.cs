using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ShadowMeshEffectBundle : EffectBundle
    {
        [SerializeField] private ParticleSystem _shadowMeshParticleEffect;
        [SerializeField] private Transform[] _transformsToShadow;
        
        public override void Play(Vector3 position)
        {
            base.Play(position);

            foreach (Transform t in this._transformsToShadow)
            {
                ParticleSystem system = Instantiate(this._shadowMeshParticleEffect, t);
                system.transform.localScale = t.lossyScale;
                ParticleSystemRenderer renderer = system.GetComponent<ParticleSystemRenderer>();
                renderer.SetMeshes(new Mesh[] {t.GetComponent<MeshFilter>().mesh});
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBundle : MonoBehaviour
{
    [SerializeField] public AudioClip Sounds;
    [SerializeField] public float Volume;
    [SerializeField] public ParticleSystem Particles;

    [SerializeField] public string _debounceGroup = "";
    [SerializeField] public float DebounceSound = 0.1f;

    [SerializeField] private bool _shouldShakeScreen = false;

    private static Dictionary<string, float> debounces = new();

    private void Start()
    {
        if (this._debounceGroup == "")
        {
            this._debounceGroup = (Random.value * 8).ToString();
        }

        if (!debounces.ContainsKey(this._debounceGroup))
        {
            debounces.Add(this._debounceGroup, 0);
        }
    }

    public virtual void Play(Vector3 position)
    {
        float lastPlayed = debounces.GetValueOrDefault(this._debounceGroup, 0);
        if (this.Sounds && (this.DebounceSound <= 0 || Time.time - lastPlayed > this.DebounceSound))
        {
            AudioHelper.PlayClip2D(this.Sounds, 1.0f);
            debounces[this._debounceGroup] = Time.time;
        }

        if (this.Particles)
        {
            Instantiate(this.Particles, position, Quaternion.identity);
        }

        if (this._shouldShakeScreen)
        {
            MainCamera.Shake();
        }
    }
}

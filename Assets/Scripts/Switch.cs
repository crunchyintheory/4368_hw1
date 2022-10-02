using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour, IDamageable
{
    [SerializeField] private MeshRenderer _colorMesh;
    [SerializeField] private EffectBundle _effectBundle;
    private Material _material;

    public event IDamageable.DamagedEventHandler OnDamaged;
    
    // Start is called before the first frame update
    void Start()
    {
        this._material = this._colorMesh.material;
        SetColor();
        Barrier.OnSwitch += SetColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetColor()
    {
        this._material.SetVector("_EmissiveColor", Barrier.GroupColors[Barrier.ActiveGroup]);
    }
    
    public void TakeDamage(int damage, DamageSource source)
    {
        this.OnDamaged?.Invoke(this, source);
        Barrier.Switch();
        this._effectBundle.Play(this.transform.position);
    }
}

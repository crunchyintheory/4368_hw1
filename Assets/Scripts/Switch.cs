using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : Damageable
{
    [SerializeField] private MeshRenderer _colorMesh;
    private Material _material;
    
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
    
    public override void ModifyHealth(float delta, DamageSource source)
    {
        Barrier.Switch();
    }
}

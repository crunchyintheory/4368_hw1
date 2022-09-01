using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    [SerializeField] private Transform _art;

    private void FixedUpdate()
    {
        this._art.transform.Rotate(Vector3.forward * -0.5f);
    }
}

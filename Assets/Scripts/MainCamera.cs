using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    private Vector3 _initialDelta;

    private void Awake()
    {
        this._initialDelta = this.transform.position - this._followTarget.position;
    }

    private void Update()
    {
        this.transform.position = this._initialDelta + this._followTarget.position;
    }
    
}

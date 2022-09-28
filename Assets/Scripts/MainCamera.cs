using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    private Vector3 _initialDelta;
    private Animator _animator;

    public static MainCamera Camera { get; private set; }
    
    public static void Shake()
    {
        Camera._animator.SetTrigger("Shake");
    }

    private void Start()
    {
        if(Camera)
            Destroy(this.gameObject);

        Camera = this;

        this._animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        this._initialDelta = this.transform.position - this._followTarget.position;
    }

    private void Update()
    {
        this.transform.position = this._initialDelta + this._followTarget.position;
    }

}

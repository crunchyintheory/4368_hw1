using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TankController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = .25f;
    [SerializeField] private float _turnSpeed = 2f;
    [SerializeField] private float _maxSpeed = 0.25f;
    [SerializeField] private float _minSpeed = 0.25f;
    [SerializeField] private float _acceleration = 0.05f;
    [SerializeField] private float _speedParticlesThreshold = 0.26f;

    [SerializeField] private ParticleSystem _speedParticles;
    [SerializeField] private Transform _bodyPivot;
    [SerializeField] private Health _health;

    public float MaxSpeed
    {
        get => this._maxSpeed;
        set
        {
            if (value >= this._speedParticlesThreshold)
            {
                this._speedParticles.Play();
            }
            else
            {
                this._speedParticles.Stop();
            }
            this._maxSpeed = value;
        }
    }

    private Rigidbody _rb;
    private AudioSource _movementAudio;
    private CharacterController _character;

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody>();
        this._movementAudio = GetComponent<AudioSource>();
        this._character = GetComponent<CharacterController>();
        this._health = GetComponentInChildren<Health>();
        this._health.OnKilled += (sender, touched) => Destroy(this.gameObject);
    }

    private void Update()
    {
        MoveTank();
        //TurnTank();
        // temporary
        //this._moveSpeed = this._maxSpeed;
        //ScaleMoveSpeed();

        float inputMagnitude = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")), 1).sqrMagnitude / 2;
        this._movementAudio.volume = inputMagnitude;
        var emission = this._speedParticles.emission;
        emission.rateOverTime = 30f * inputMagnitude;
    }

    // Not sure why max speed does nothing here, but this fixes it somewhat
    // Will implement this in the future to make sure the speed system is immediately apparent to be functioning
    /*public void ScaleMoveSpeed()
    {
        float acceleration = this._acceleration;
        float movementMagnitude = Mathf.Abs(Input.GetAxis("Vertical"));
        // a bit convoluted, but we do this to avoid any floating point issues in just checking == 0
        if (movementMagnitude < 0.01)
        {
            acceleration = -acceleration * 3;
        }

        acceleration *= Time.deltaTime;
        
        this._moveSpeed = Mathf.Clamp(this._moveSpeed + acceleration, this._minSpeed, this._maxSpeed);
        Debug.Log(acceleration);
    }*/

    public void MoveTank()
    {
        Vector3 moveInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1);
        Vector3 moveAmountThisFrame = moveInput * this._moveSpeed;
        this._character.Move((moveAmountThisFrame + Physics.gravity) * Time.deltaTime);

        /*if(Input.GetAxis("Vertical") > 0)
            this._bodyPivot.rotation = Quaternion.Euler(Vector3.Cross(Vector3.forward, moveInput) * 90);
        else
            this._bodyPivot.rotation = Quaternion.Euler(Vector3.Cross(Vector3.back, moveInput) * 90);*/
    }
}

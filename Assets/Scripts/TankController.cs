using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = .25f;
    [SerializeField] private float _turnSpeed = 2f;
    [SerializeField] private float _maxSpeed = 0.25f;
    [SerializeField] private float _minSpeed = 0.25f;
    [SerializeField] private float _acceleration = 0.05f;

    public float MaxSpeed
    {
        get => this._maxSpeed;
        set => this._maxSpeed = value;
    }

    private Rigidbody _rb = null;

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveTank();
        TurnTank();
        // temporary
        this._moveSpeed = this._maxSpeed;
        //ScaleMoveSpeed();
    }

    // Not sure why max speed does nothing here, but this fixes it somewhat
    // Will implement this in the future to make sure the speed system is immediately apparent to be functioning
    public void ScaleMoveSpeed()
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
    }

    public void MoveTank()
    {
        // calculate the move amount
        float moveAmountThisFrame = Input.GetAxis("Vertical") * this._moveSpeed;
        // create a vector from amount and direction
        Vector3 moveOffset = this.transform.forward * moveAmountThisFrame;
        // apply vector to the rigidbody
        this._rb.MovePosition(this._rb.position + moveOffset);
        // technically adjusting vector is more accurate! (but more complex)
    }

    public void TurnTank()
    {
        // calculate the turn amount
        float turnAmountThisFrame = Input.GetAxis("Horizontal") * this._turnSpeed;
        // create a Quaternion from amount and direction (x,y,z)
        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        // apply quaternion to the rigidbody
        this._rb.MoveRotation(this._rb.rotation * turnOffset);
    }
}

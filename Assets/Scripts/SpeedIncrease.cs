using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedIncrease : CollectibleBase
{
    [SerializeField] private float _speedAmount = 0.25f;

    protected override void Collect(Player player)
    {
        //pull motor controller from the player
        TankController controller = player.GetComponent<TankController>();
        if (controller != null)
        {
            controller.MaxSpeed += this._speedAmount;
        }
    }

    protected override void Movement(Rigidbody rb)
    {
        //calculate rotation
        Quaternion turnOffset = Quaternion.Euler(this.MovementSpeed, this.MovementSpeed, this.MovementSpeed);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}

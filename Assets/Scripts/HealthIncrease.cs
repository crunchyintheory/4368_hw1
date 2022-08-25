using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthIncrease : CollectibleBase
{
    [SerializeField] private int _healthAdded = 1;

    protected override void Collect(Player player)
    {
        player.IncreaseHealth(this._healthAdded);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Treasure : CollectibleBase
{
    [SerializeField] private int _value = 1;

    protected override void Collect(Player player)
    {
        //pull motor controller from the player
        player.GetTreasure(this._value);
    }
}

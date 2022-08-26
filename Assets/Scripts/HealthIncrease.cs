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
    
    protected override void Feedback(Player player)
    {
        // particles
        if (this.CollectParticles != null)
        {
            Instantiate(this.CollectParticles, player.transform);
        }
        //audio. TODO - consider Obejct (sic) Pooling for performance
        if (this.CollectSound != null)
        {
            AudioHelper.PlayClip2D(this.CollectSound, 1f);
        }
    }
}

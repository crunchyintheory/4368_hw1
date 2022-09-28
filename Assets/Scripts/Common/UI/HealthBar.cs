using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject[] _targets; // Unity no likey the interface, assumed to be IKillable

    private IKillable[] _killables;

    private RectTransform _rect;

    private float _baseWidth;
    private float _healthTickWidth;

    // Start is called before the first frame update
    private void Start()
    {
        this._rect = GetComponent<RectTransform>();
        this._baseWidth = this._rect.rect.width;
        this._killables = new IKillable[this._targets.Length];

        float cumulativeMaxHealth = 0;
        
        for (int i = 0; i < this._targets.Length; i++)
        {
            this._killables[i] = this._targets[i].GetComponent<IKillable>();
            cumulativeMaxHealth += this._killables[i].MaxHealth;
            this._killables[i].OnDamaged += OnTargetDamaged;
        }

        this._healthTickWidth = this._baseWidth / cumulativeMaxHealth;
    }

    public int ComputeCurrentCumulativeHealth()
    {
        int health = 0;
        foreach (IKillable killable in _killables)
        {
            health += killable.CurrentHealth;
        }

        return health;
    }

    private void OnTargetDamaged(IDamageable sender, DamageSource source)
    {
        this._rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComputeCurrentCumulativeHealth() * this._healthTickWidth);
    }
}

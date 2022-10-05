using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }

    private void OnEnable()
    {
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

    private void OnDisable()
    {
        for (int i = 0; i < this._targets.Length; i++)
        {
            this._killables[i].OnDamaged -= OnTargetDamaged;
        }        
    }

    public int ComputeCurrentCumulativeHealth()
    {
        return this._killables.Sum(killable => killable.CurrentHealth);
    }

    private void OnTargetDamaged(IDamageable sender, DamageSource source)
    {
        this._rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComputeCurrentCumulativeHealth() * this._healthTickWidth);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    private TankController _tankController;

    private void Awake()
    {
        this._tankController = GetComponent<TankController>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this._currentHealth = this._maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHealth(int amount)
    {
        this._currentHealth = Mathf.Clamp(this._currentHealth + amount, 0, this._maxHealth);
        Debug.Log($"Player's health: {this._currentHealth}");
    }

    public void DecreaseHealth(int amount)
    {
        this._currentHealth -= amount;
        Debug.Log($"Player's health: {this._currentHealth}");
        if (this._currentHealth <= 0)
        {
            this.Kill();
        }
    }

    public void Kill()
    {
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;
    private int _treasureCount = 0;

    private TankController _tankController;

    [Header("References")] [SerializeField]
    private TextMeshProUGUI _treasureText;

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

    public void GetTreasure(int value = 1)
    {
        this._treasureCount += value;
        this._treasureText.text = $"Treasure: {this._treasureCount}";
    }
}

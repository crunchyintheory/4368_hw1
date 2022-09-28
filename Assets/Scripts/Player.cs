using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class Player : MonoBehaviour, ITeamable
{
    public int Team
    {
        get => 0;
    }
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;
    private int _treasureCount = 0;
    private bool _invincible = false;

    public bool Invincible
    {
        get => this._invincible;
        set
        {
            this._shieldBubble.SetActive(value);
            this._invincible = value;
        }
    }

    private TankController _tankController;

    [Header("References")] [SerializeField]
    private TextMeshProUGUI _treasureText;

    [SerializeField] private GameObject _shieldBubble;

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

    // IMPORTANT: The player damage script is NOT in this file, see TankHealth gameobject in the prefab!
    // This code is here for legacy purposes and is subject to removal!
    public void IncreaseHealth(int amount)
    {
        this._currentHealth = Mathf.Clamp(this._currentHealth + amount, 0, this._maxHealth);
        Debug.Log($"Player's health: {this._currentHealth}");
    }

    public void DecreaseHealth(int amount)
    {
        if (this.Invincible) return;
        this._currentHealth -= amount;
        Debug.Log($"Player's health: {this._currentHealth}");
        if (this._currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (this.Invincible) return;
        this.gameObject.SetActive(false);
    }

    public void GetTreasure(int value = 1)
    {
        this._treasureCount += value;
        this._treasureText.text = $"Treasure: {this._treasureCount}";
    }
}

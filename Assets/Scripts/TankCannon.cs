using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    [SerializeField] private Transform _cannonPivot;
    [SerializeField] private float _shotCooldown = 0.5f;
    [SerializeField] private ProjectileMovement _shot;
    [SerializeField] private Transform _shotSpawnPosition;
    [SerializeField] private Transform _shotEffectPosition;

    [SerializeField] private EffectBundle _shotEffects;

    private Plane _groundPlane = new Plane(Vector3.up, 0);

    private float _lastShot = 0;
    private Health _player;
    
    // Start is called before the first frame update
    void Awake()
    {
        this._player = GetComponentInChildren<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        this._groundPlane.distance = this.transform.position.y;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (this._groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            point.y = this._cannonPivot.transform.position.y;
            this._cannonPivot.transform.LookAt(point);
        }

        if (Input.GetKeyUp(KeyCode.Space) && Time.time - this._lastShot >= this._shotCooldown)
        {
            this._lastShot = Time.time;
            ProjectileMovement shot = Instantiate(this._shot, this._shotSpawnPosition.position, this._cannonPivot.rotation);
            shot.GetComponent<DamageSource>().Team = this._player.Team;
            shot.InitialVelocity = new Vector3(0, 0, 8);
            
            // Spawn Effects
            this._shotEffects.Play(this._shotEffectPosition.position);
        }
    }
}

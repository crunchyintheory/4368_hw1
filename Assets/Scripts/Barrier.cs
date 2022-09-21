using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Barrier : MonoBehaviour
{
    public static List<Barrier> Barriers { get; private set; }
    public static short ActiveGroup { get; private set; } = 0;
    public const short NumGroups = 2;
    public static Color[] GroupColors;
    
    public delegate void SwitchEventHandler();
    public static event SwitchEventHandler OnSwitch;

    public static void Switch()
    {
        if (++ActiveGroup >= NumGroups)
            ActiveGroup = 0;

        Barriers.ForEach((Barrier barrier) =>
        {
            barrier.SetActive(barrier._group == ActiveGroup);
        });
        OnSwitch?.Invoke();
    }

    public static void RegisterBarrier(Barrier barrier)
    {
        Barriers ??= new List<Barrier>();
        Barriers.Add(barrier);
        
        barrier.SetActive(barrier._group == ActiveGroup);
    }

    [SerializeField] private short _group;
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float _animationSpeed = 1f;

    [Header("Colors - HDR Hack")] 
    [SerializeField, ColorUsage(false, true)] private Color _color1;
    [SerializeField, ColorUsage(false, true)] private Color _color2;

    private Vector3 _basePosition;
    private Coroutine _animation;
    private bool _active;

    private Collider _collider;

    private void Awake()
    {
        GroupColors ??= new Color[] {this._color1, this._color2};
        this._collider = GetComponent<Collider>();
        this._basePosition = this.transform.position;
        RegisterBarrier(this);
    }

    private void Start()
    {
        Material m = GetComponent<MeshRenderer>().materials[0];
        m.SetVector("_EmissiveColor", GroupColors[this._group]);
    }
    
    public void SetActive(bool active)
    {
        this._active = active;
        if(this._animation != null)
            StopCoroutine(this._animation);
        this._collider.enabled = active;
        this._animation = StartCoroutine(ActivationAnimation(active));
    }

    private IEnumerator ActivationAnimation(bool forwards = true)
    {
        for (float i = 0; i < 1; i+=Time.fixedDeltaTime * this._animationSpeed)
        {
            // Play animation backwards if we are going back up.
            float delta = forwards ? this._movementCurve.Evaluate(1 - i) : this._movementCurve.Evaluate(i);
            this.transform.position = this._basePosition + Vector3.down * delta;
            yield return new WaitForFixedUpdate();
        }
    }

}

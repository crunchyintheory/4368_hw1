public interface IKillable : IDamageable
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }

    public delegate void KilledEventHandler(IKillable sender, DamageSource source);

    public event KilledEventHandler OnKilled;
}
public interface IDamageable
{
    public void TakeDamage(int damage, DamageSource source);
    
    public delegate void DamagedEventHandler(IDamageable sender, DamageSource source);
    public event DamagedEventHandler OnDamaged;
}
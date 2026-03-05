namespace EligosNewGame
{

    public interface IDamageable
    {
        public void TakeDamage(int damage);
    }

    public interface IWeapon
    {
        public void Attack(IDamageable target);
    }

    public class Player : IDamageable
    {
        public IWeapon Weapon { get; set; }

        public IDamageable Target { get; set; }

        public int Health { get; set; }
        public int Power { get; set; }

        public void Attack()
        {
            Weapon.Attack(Target);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }

    public class Weapon : IWeapon
    {
        public int Damage { get; set; }

        public void Attack(IDamageable target)
        {
            target.TakeDamage(Damage);
        }
    }

    public class Enemy : IDamageable
    {
        public int Health { get; set; }
        public int Power { get; set; }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }

    public class Bow : Weapon
    {
        public int Damage { get; set; } = 10;
    }
}
namespace Project.Scripts.Game.Gameplay.Enemies
{
    public class Health
    {
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        public Health(int maxHealth)
        {
            CurrentHealth = maxHealth;
        }

        public void Reduce(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }
    }
}
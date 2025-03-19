namespace Project.Scripts.Game.Gameplay.Player
{
    public interface IPlayerHealth
    {
        void TakeDamage(int damage);
        void SaveHealth();
    }
}
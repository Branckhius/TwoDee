using Project.Scripts.DataCore;
using Project.Scripts.DataCore.DataStructure;
using Project.Scripts.Game.Gameplay.Enemies;
using UnityEngine;
using VContainer;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable,IPlayerHealth
    {
        [Header("Health Settings")] public int maxHealth ;
        private int currentHealth;
        private Animator animator;



        void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();

        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}");

            SaveHealth(); // Salvăm noul HP

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void SaveHealth()
        {
            var gameData = new GameData(new PlayerRelatedData { _currentHelath = currentHealth });
            gameData.playerRelatedData._currentHelath=currentHealth;

            Debug.Log("Player HP saved!");
        }

        private void Die()
        {
            animator.SetTrigger("Death");
            Debug.Log("Player is Dead!");
            gameObject.SetActive(false);
        }
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp((int)health, 0, maxHealth);
            Debug.Log($"Player health set to {currentHealth}");
        }
    }
}
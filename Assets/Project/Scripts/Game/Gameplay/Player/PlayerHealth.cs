using System;
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
        public ILocalDataStorage _localDataStorage;
        

        public HealthBar healthBar;

        void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            healthBar.SetMaxHealth(maxHealth);
            if (_localDataStorage.Has())
            {
                GameData gameData =_localDataStorage.Fetch();
                currentHealth = Convert.ToInt32(gameData.playerRelatedData._currentHealth);            }
            else
            {
                currentHealth = maxHealth;
            }
            
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);

            Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}");

            SaveHealth(); // Salvăm noul HP

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void SaveHealth()
        {
            var gameData = new GameData(new PlayerRelatedData { _currentHealth = currentHealth });
            gameData.playerRelatedData._currentHealth=currentHealth;
            _localDataStorage.Store(gameData);
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
            healthBar?.SetHealth(currentHealth);
        }
    }
}
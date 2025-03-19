using UnityEngine;

namespace Project.Scripts.Game.Gameplay
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Gameplay Scope Configuration")]
    public class GameplayScopeConfiguration : ScriptableObject
    {
        public GameObject GameplayBG;
        public GameObject Floor;
        public GameObject Player;
        public GameObject Enemy;
        public GameObject Floor2;
        public GameObject JumpPlace;
        public GameObject DownPlace;
        public GameObject Trick;
        public GameObject Glock;
        public GameObject GlockBullet;
        public GameObject PlayerHealth;
        public GameObject EnemyHealth;


    }
}
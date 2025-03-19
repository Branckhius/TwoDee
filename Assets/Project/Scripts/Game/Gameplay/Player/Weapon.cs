using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public abstract class Weapon : MonoBehaviour
    {
        // Metoda Fire() va fi implementată de fiecare armă specifică.
        public virtual async UniTask Fire() { await UniTask.CompletedTask; }
        public virtual async UniTask Reload() { await UniTask.CompletedTask; }
    }
}
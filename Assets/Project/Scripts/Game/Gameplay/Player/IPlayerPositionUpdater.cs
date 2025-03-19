using Project.Scripts.Game.UI.SceneSystem;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public interface IPlayerPositionUpdater
    {
        void Initialize(GameplayScopeConfiguration gameplayConfig, BaseScene scene, Camera gameCamera);
        void UpdatePlayerPosition();
    }
}
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Floors
{
    public class FloorManager
    {
        private Transform _sceneTransform;
        private GameplayScopeConfiguration _gameplayConfig;

        public FloorManager(Transform sceneTransform, GameplayScopeConfiguration gameplayConfig)
        {
            _sceneTransform = sceneTransform;
            _gameplayConfig = gameplayConfig;
        }

        public GameObject CreateFloor(GameObject prefab, string layer)
        {
            GameObject floor = Object.Instantiate(prefab, _sceneTransform);
            floor.layer = LayerMask.NameToLayer(layer);
            return floor;
        }
    }
}
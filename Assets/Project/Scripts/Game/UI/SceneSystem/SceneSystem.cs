using System;
using LWFlo.Tools;
using Project.Scripts.Game.Data;
using Project.Scripts.Game.GameManager;
using VContainer;
using Object = UnityEngine.Object;

namespace Project.Scripts.Game.UI.SceneSystem
{
    public class SceneSystem : ISceneSystem
    {
        private readonly IGameManager _gameManager;
        private readonly GameConfiguration _config;
        
        [Inject]
        public SceneSystem(GameConfiguration config, IGameManager gameManager)
        {
            _config = config;
            _gameManager = gameManager;
        }
        
        public BaseScene GetScene()
        {
            var currentState = _gameManager.GetBreadcrumbs();
            
            try
            {
                var stateName = currentState.TryConvertToEnumString<GameStateNames>();
                var scene = _config.sceneData[stateName];
                var instantiatedScene = Object.Instantiate(scene);
                return instantiatedScene;
            }
            catch (Exception e)
            {
                throw new Exception($"[{nameof(SceneSystem)}] :  I cannot find {currentState}, you should have check in {nameof(GameConfiguration)} Scene Data");
            }
        }
    }
}
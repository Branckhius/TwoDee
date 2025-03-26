using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.Constants;
using Project.Scripts.Game.UI.SceneSystem;
using Project.Scripts.Messages.Requests.Game;
using Project.Scripts.Game.Gameplay;
using UnityEngine;
using VContainer;
using Project.Scripts.DataCore;
using Project.Scripts.DataCore.DataStructure;
using Project.Scripts.Game.Gameplay.Cameras;
using Project.Scripts.Game.Gameplay.CanvasBG;
using Project.Scripts.Game.Gameplay.Floors;
using Project.Scripts.Game.Gameplay.NoExitBorders;
using Project.Scripts.Game.Gameplay.Player;
using VContainer.Unity;

namespace Project.Scripts.Game.GameManager.States
{
    public class GameStateGameplay : GameStateBase<GameStateGameplay.Context>
    {
        public class Context
        {
            public GameData _gameDataa { get; }

            public Context(GameData gameDataContext)
            {
                _gameDataa = gameDataContext;
            }
        }

        private readonly LifetimeScope _currentScope;
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly ISceneSystem _sceneSystem;
        
        private Context _currentContext;
        private GameData _gameData;
        private PlayerHealth _playerHealth;
        private readonly ILocalDataStorage _localDataStorage;

        [Inject]
        public GameStateGameplay(LifetimeScope currentScope, 
            IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, 
            ISceneSystem sceneSystem,ILocalDataStorage localDataStorage, Context context
        )
        {
            _currentScope = currentScope;
            _scopeCreator = scopeCreator;
            _sceneSystem = sceneSystem;
            _localDataStorage = localDataStorage;
            _gameData = context._gameDataa;
        }

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            Debug.Log("Suntem in Gameplay");

            var scopeResult = _scopeCreator.Invoke(new CreateScopeRequest
            {
                childName = ScopeNames.GAMEPLAY_SCOPE,
                parentScope = _currentScope,
            });
            _gameData = Context._gameDataa;

            var scope = scopeResult.childScope;
            Debug.Log(scope + " scope scope");

            var gameplayScope = scope as GameplayScope;
            var gameplayConfig = gameplayScope.Container.Resolve<GameplayScopeConfiguration>();
            var scene = _sceneSystem.GetScene();
            
            GameObject player = Object.Instantiate(gameplayConfig.Player, scene.transform);
            player.layer = LayerMask.NameToLayer("Player");
            _playerHealth = player.GetComponent<PlayerHealth>();
            
            if (_playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found!");
                return;
            }
            // Folosim GameData transmis prin context
            if (_gameData != null)
            {
                Debug.Log("Intra resume");
                ResumeGame();
            }
            else
            {
                Debug.Log("Intra new");

                StartNewGame();
            }

            CameraManager cameraManager = new CameraManager();
            cameraManager.InitializeCameras(scene.transform, gameplayConfig.Player);
            cameraManager.ConfigureSplashScreen();

            // Creare și inițializare CanvasManager pentru Background
            CanvasManager canvasManager = new CanvasManager(scene.transform, cameraManager.GameCamera);
            canvasManager.CreateGameplayCanvas(gameplayConfig.GameplayBG, gameplayConfig.PlayerHealth, player);

            GameObject floor = Object.Instantiate(gameplayConfig.Floor, scene.transform);
            floor.layer = LayerMask.NameToLayer("Ground");

            FloorPositionUpdater floorUpdater = floor.AddComponent<FloorPositionUpdater>();
            floorUpdater.Initialize(gameplayConfig, scene, cameraManager.GameCamera);

            PlayerPositionUpdater playerUpdater = player.AddComponent<PlayerPositionUpdater>();
            playerUpdater.Initialize(gameplayConfig, scene, cameraManager.GameCamera);

            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController._gameplayConfig = gameplayConfig;
            playerController._camera = cameraManager.GameCamera;
            
            GameObject noExit = Object.Instantiate(gameplayConfig.Floor, scene.transform);

            // Adăugare FloorPositionUpdater
            NoExitUpdater noExitUpdater = noExit.AddComponent<NoExitUpdater>();
            noExitUpdater.GameplayConfig = gameplayConfig;
            noExitUpdater.cameraTransform = cameraManager.GameCamera.transform;
            noExitUpdater.cameraHeight = cameraManager.GameCamera.orthographicSize * 2f;
            noExitUpdater.Scene = scene;
            noExitUpdater.camera = cameraManager.GameCamera;
        }
        private void ResumeGame()
        {
            Debug.Log("Resuming game...");
            _playerHealth.SetHealth(_gameData.playerRelatedData._currentHelath);
        }

        private void StartNewGame()
        {
            Debug.Log("Starting new game...");
            _gameData = new GameData
            {
                playerRelatedData = new PlayerRelatedData
                {
                    _currentHelath = 100,
                    attackDamage = 10
                }
            };
        }



        protected override void OnSuspend()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnResume()
        {
            throw new System.NotImplementedException();
        }
    }
}

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
            private GameData _gameData;
            public class Context
            {
                public GameData GameData { get; }

                public Context(GameData gameData)
                {
                    GameData = gameData;
                }
            }

            private readonly LifetimeScope _currentScope;
            private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
            private readonly ISceneSystem _sceneSystem;
            

            private PlayerHealth _playerHealth;
            private readonly ILocalDataStorage _localDataStorage;

            [Inject]
            public GameStateGameplay(LifetimeScope currentScope, 
                IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, 
                ISceneSystem sceneSystem,ILocalDataStorage localDataStorage,GameStateGameplay.Context context
            )
            {
                _currentScope = currentScope;
                _scopeCreator = scopeCreator;
                _sceneSystem = sceneSystem;
                _localDataStorage = localDataStorage;
                _gameData = context.GameData;

            }

            protected override async UniTask OnRun(CancellationToken cancellationToken)
            {
                Debug.Log("Suntem in Gameplay");
                await SetScreenOrientation(ScreenOrientation.LandscapeLeft, cancellationToken);


                var scopeResult = _scopeCreator.Invoke(new CreateScopeRequest
                {
                    childName = ScopeNames.GAMEPLAY_SCOPE,
                    parentScope = _currentScope,
                });

                var scope = scopeResult.childScope;

                var gameplayScope = scope as GameplayScope;
                var gameplayConfig = gameplayScope.Container.Resolve<GameplayScopeConfiguration>();
                var scene = _sceneSystem.GetScene();
                
                GameObject player = Object.Instantiate(gameplayConfig.Player, scene.transform);
                player.layer = LayerMask.NameToLayer("Player");
                _playerHealth = player.GetComponent<PlayerHealth>();
                _playerHealth._localDataStorage = _localDataStorage;
                Debug.Log($"Health curent: {_gameData.playerRelatedData._currentHealth}");

                
                

                // Folosim GameData transmis prin context
                if (_gameData.playerRelatedData._currentHealth <= 0)
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
                canvasManager.CreateGameplayCanvas(gameplayConfig.GameplayBG, gameplayConfig.PlayerHealth, gameplayConfig.Joystick, gameplayConfig.JumpButton
                    , gameplayConfig.jump_circle_active, gameplayConfig.jump_circle_used, gameplayConfig.AmmoOnScreen, player);

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
                _playerHealth.SetHealth(_gameData.playerRelatedData._currentHealth);
            }

            private void StartNewGame()
            {
                Debug.Log("Starting new game...");
                _gameData = new GameData
                {
                    playerRelatedData = new PlayerRelatedData
                    {
                        _currentHealth = 150,
                        attackDamage = 10
                    }
                };
            }

            private async UniTask SetScreenOrientation(ScreenOrientation targetOrientation, CancellationToken cancellationToken)
            {
                Screen.orientation = targetOrientation;

                // Așteaptă până când orientarea este efectiv aplicată
                while (Screen.orientation != targetOrientation)
                {
                    await UniTask.Yield(cancellationToken); // Așteaptă un frame
                }
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

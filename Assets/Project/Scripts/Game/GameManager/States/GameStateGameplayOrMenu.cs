using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Scripts.DataCore;
using Project.Scripts.DataCore.DataStructure;
using Project.Scripts.Game.Gameplay.Player;
using UnityEngine;
using VContainer;

namespace Project.Scripts.Game.GameManager.States
{
    public class GameStateGameplayOrMenu : GameStateBase<GameStateGameplayOrMenu.Context>
    {
        private readonly ILocalDataStorage _localDataStorage;
        private readonly IGameManager _gameManager;
        private readonly IObjectResolver _resolver;
        GameData gameData;
        
        [Inject]
        public GameStateGameplayOrMenu(IObjectResolver resolver,ILocalDataStorage localDataStorage, IGameManager gameManager)
        {
            _localDataStorage = localDataStorage;
            _gameManager = gameManager;
            _resolver = resolver;
        }

        public class Context {}

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            
            if (_localDataStorage.Has()) // Dacă există salvare, reia jocul
            {
                Debug.Log("a intrat .has");
                GameData gameData =_localDataStorage.Fetch();

                _resolver.Inject(gameData);
                var gameplayContext = new GameStateGameplay.Context(gameData);

                _gameManager.EnqueueSwitchState<GameStateGameplay, GameStateGameplay.Context>(gameplayContext);
                    
            }
            else 
            {
                //GameData gameData = new GameData();
                //var gameplayContext = new GameStateGameplay.Context(gameData);
                //_gameManager.EnqueueSwitchState<GameStateGameplay, GameStateGameplay.Context>(gameplayContext);
                var menuContext = new GameStateMenu.Context();

                _gameManager.EnqueueSwitchState<GameStateMenu, GameStateMenu.Context>(menuContext);

            }
        }


        
        protected override void OnSuspend()
        {
            throw new NotImplementedException();
        }

        protected override void OnResume()
        {
            throw new NotImplementedException();
        }
    }
}
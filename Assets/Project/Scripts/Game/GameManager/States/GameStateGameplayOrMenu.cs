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
        
        [Inject]
        public GameStateGameplayOrMenu(ILocalDataStorage localDataStorage, IGameManager gameManager)
        {
            _localDataStorage = localDataStorage;
            _gameManager = gameManager;
        }

        public class Context {}

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            if (_localDataStorage.Has()) // Dacă există salvare, reia jocul
            {
                GameData gameData = _localDataStorage.Has() ? _localDataStorage.Fetch() : null;
                Debug.Log($"Loaded Player HP: {gameData.playerRelatedData._currentHelath}");

                var gameplayContext = new GameStateGameplay.Context(gameData);
                _gameManager.EnqueueSwitchState<GameStateGameplay, GameStateGameplay.Context>(gameplayContext);
            }
            else // Dacă nu există salvare, începe un joc nou
            {
                GameData gameData = new GameData();
                var gameplayContext = new GameStateGameplay.Context(gameData);
                _gameManager.EnqueueSwitchState<GameStateGameplay, GameStateGameplay.Context>(gameplayContext);
                //var menuContext = new GameStateMenu.Context();
                //_gameManager.EnqueueSwitchState<GameStateMenu, GameStateMenu.Context>(menuContext);

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
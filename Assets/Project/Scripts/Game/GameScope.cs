using MessagePipe;
using Project.Scripts.DataCore;
using Project.Scripts.Game.GameManager;
using Project.Scripts.Game.GameManager.States;
using Project.Scripts.Game.UI.SceneSystem;
using Project.Scripts.Messages.Requests.Events;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Game
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private GameConfiguration _config;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LocalDataStorage>(Lifetime.Singleton)
                .As<ILocalDataStorage>();
            builder.Register<SceneSystem>(Lifetime.Singleton)
                .WithParameter(_config)
                .As<ISceneSystem>();
            
            builder.Register<GameStateGameplayOrMenu>(Lifetime.Transient);
            builder.Register<GameStateMenu>(Lifetime.Transient);
            builder.Register<GameStateGameplay>(Lifetime.Transient);
            
            var messageOptions = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<GameStateChangedMessage>(messageOptions);
            
            builder.Register<GameManager.GameManager>(Lifetime.Singleton)
                .As<IGameManager>();
            
        }
    }
}
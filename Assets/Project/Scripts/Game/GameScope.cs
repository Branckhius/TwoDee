﻿using Project.Scripts.DataCore;
using Project.Scripts.Game.GameManager;
using Project.Scripts.Game.GameManager.States;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Game
{
    public class GameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LocalDataStorage>(Lifetime.Singleton)
                .As<ILocalDataStorage>();
            
            builder.Register<GameStateGameplayOrResume>(Lifetime.Transient);
            
            builder.Register<GameManager.GameManager>(Lifetime.Singleton)
                .As<IGameManager>();
        }
    }
}
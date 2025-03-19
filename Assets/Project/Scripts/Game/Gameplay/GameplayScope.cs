using Project.Scripts.Game.GameManager.States;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Game.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private GameplayScopeConfiguration _gameplayConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameplayConfig)
                .As<GameplayScopeConfiguration>()
                .AsSelf();
        }
    }
}
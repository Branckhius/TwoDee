using Project.Scripts.Game.GameManager.States;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Game.Menu
{
    public class MenuScope : LifetimeScope
    {
        [SerializeField] private MenuScopeConfiguration _menuConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_menuConfig)
                .As<MenuScopeConfiguration>()
                .AsSelf();
            
        }
    }
}
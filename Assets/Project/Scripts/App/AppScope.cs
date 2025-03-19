﻿using Project.Scripts.App.Scope;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.App
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private AppScopeConfiguration _config;

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            base.Awake();
        }

        private void Start()
        {
            AppStartup();
        }

        private void AppStartup()
        {
            // Entry point - where the adventure begins
            var appManager = Container.Resolve<AppManager>();
            appManager.Start();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Application.targetFrameRate = 60;
            
            builder.Register<ChildScopeService>(Lifetime.Singleton)
                .WithParameter(_config.childScopeConfiguration)
                .AsImplementedInterfaces();

            builder.Register<AppManager>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}
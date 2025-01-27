using System;
using MessagePipe;
using Project.Scripts.Messages.Requests.Game;
using VContainer.Unity;

namespace Project.Scripts.App.Scope
{
    public interface IChildScopeService :         
        IRequestHandler<CreateScopeRequest, CreateScopeResponse>,
        IRequestHandler<FetchScopeRequest, FetchScopeResponse>,
        IRequestHandler<DisposeScopeRequest, DisposeScopeResponse>
    {
        LifetimeScope CreateChildScope(LifetimeScope parentScope, string childName, Action<LifetimeScope> setDynamicConfigMethodPreBuild, Action<LifetimeScope> setDynamicConfigMethodPostBuild);
        bool DisposeScope(string childName, bool throwIfNotFound);
    }
}
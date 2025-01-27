using VContainer.Unity;

namespace Project.Scripts.Messages.Requests.Game
{
    public struct FetchScopeRequest
    {
        public string scopeName;
    }

    public struct FetchScopeResponse
    {
        public LifetimeScope scope;
    }
}
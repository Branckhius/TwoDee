using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.App.Scope
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Child Scope Configuration")]
    public class ChildScopeConfiguration : ScriptableObject
    {
        public LifetimeScope[] scopes;
    }
}
using Project.Scripts.App.Scope;
using UnityEngine;

namespace Project.Scripts.App
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/App Scope Configuration")]
    public class AppScopeConfiguration : ScriptableObject
    {
        public ChildScopeConfiguration childScopeConfiguration;
    }
}
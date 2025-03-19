using Library.SerializableDictionary;
using Project.Scripts.Game.Data;
using Project.Scripts.Game.UI.SceneSystem;
using UnityEngine;

namespace Project.Scripts.Game
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        public SerializableDictionaryBase<GameStateNames, BaseScene> sceneData;
    }
}
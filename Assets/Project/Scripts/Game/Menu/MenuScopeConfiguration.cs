using UnityEngine;

namespace Project.Scripts.Game.Menu
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Menu Scope Configuration")]
    public class MenuScopeConfiguration : ScriptableObject
    {
        public GameObject backMenuScreen;
        
        public GameObject newGameLabel;
        public GameObject continueGameLabel;
        public GameObject inventoryLabel;        
        public GameObject optionsLabel;
        public GameObject exitLabel;
        
        public GameObject newGameText;
        public GameObject continueGameText;
        public GameObject inventoryText;        
        public GameObject optionsText;
        public GameObject exitText;
    }
}
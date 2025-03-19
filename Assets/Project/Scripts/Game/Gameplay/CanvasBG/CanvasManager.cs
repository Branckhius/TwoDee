using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Game.Gameplay.CanvasBG
{
    public class CanvasManager
    {
        private Transform _parent;
        private Camera _gameCamera;

        public CanvasManager(Transform parent, Camera gameCamera)
        {
            _parent = parent;
            _gameCamera = gameCamera;
        }

        public void CreateGameplayCanvas(GameObject gameplayBGPrefab, GameObject playerHealthPrefab)
        {
            // Creare Canvas pentru Background
            GameObject canvasObj = new GameObject("GameplayCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _gameCamera;

            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            canvasObj.transform.SetParent(_parent, false);

            // Instanțiere GameplayBG din prefab
            GameObject gameplayBG = GameObject.Instantiate(gameplayBGPrefab, canvasObj.transform);
    
            // Ajustare Transform pentru Stretch
            RectTransform bgRectTransform = gameplayBG.GetComponent<RectTransform>();
            if (bgRectTransform != null)
            {
                bgRectTransform.anchorMin = Vector2.zero;
                bgRectTransform.anchorMax = Vector2.one;
                bgRectTransform.offsetMin = Vector2.zero;
                bgRectTransform.offsetMax = Vector2.zero;
            }

            // Instanțiere Player Health UI în același Canvas
            GameObject playerHealthUI = GameObject.Instantiate(playerHealthPrefab, canvasObj.transform);

            // Ajustare Transform pentru poziționare corectă în UI
            RectTransform healthRectTransform = playerHealthUI.GetComponent<RectTransform>();
            if (healthRectTransform != null)
            {
                healthRectTransform.anchorMin = new Vector2(0.5f, 1f);
                healthRectTransform.anchorMax = new Vector2(0.5f, 1f);
                healthRectTransform.pivot = new Vector2(0.5f, 1f);
            }
        }

    }
}
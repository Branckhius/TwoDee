using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts
{
    public static class UICanvasManager
    {
        private static Canvas _uiCanvas;

        static UICanvasManager()
        {
            EnsureCanvasExists();
        }

        private static void EnsureCanvasExists()
        {
            _uiCanvas = GameObject.FindObjectOfType<Canvas>();

            if (_uiCanvas == null)
            {
                GameObject canvasObj = new GameObject("UICanvas");
                _uiCanvas = canvasObj.AddComponent<Canvas>();
                
                _uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    _uiCanvas.worldCamera = mainCamera;
                }
                else
                {
                    Debug.LogError("Main Camera not found! Make sure your camera has the 'MainCamera' tag.");
                }

                // Adaugă componentele necesare
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                _uiCanvas.sortingOrder = 10; // Se asigură că e deasupra altor elemente
            }
        }

        public static void AttachToCanvas(GameObject uiElement)
        {
            if (_uiCanvas != null)
            {
                uiElement.transform.SetParent(_uiCanvas.transform, false);
            }
        }
    }
}
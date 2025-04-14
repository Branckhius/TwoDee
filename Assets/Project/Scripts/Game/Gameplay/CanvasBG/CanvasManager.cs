using System.Collections.Generic;
using Project.Scripts.Game.Gameplay.Player;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Project.Scripts.Game.Gameplay.CanvasBG
{
    public class CanvasManager
    {
        private Transform _parent;
        private Camera _gameCamera;
        public List<GameObject> jumpActiveIcons = new List<GameObject>();

        public int jumpsTotal;

        public CanvasManager(Transform parent, Camera gameCamera)
        {
            _parent = parent;
            _gameCamera = gameCamera;
        }

        public void CreateGameplayCanvas(GameObject gameplayBGPrefab, GameObject playerHealthPrefab, GameObject joystick,GameObject jumpButton,GameObject jump_active,GameObject jump_used, GameObject AmmoOnScreen ,GameObject player)
        {
            // Creare Canvas pentru Background
            GameObject canvasObj = new GameObject("BackGameplayCanvas");
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

            // Creare Canvas pentru Background
            GameObject canvasObj2 = new GameObject("FrontGameplayCanvas");
            var canvas2 = canvasObj2.AddComponent<Canvas>();
            canvas2.renderMode = RenderMode.ScreenSpaceCamera;
            canvas2.worldCamera = _gameCamera;

            var canvasScaler2 = canvasObj2.AddComponent<CanvasScaler>();
            canvasScaler2.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler2.referenceResolution = new Vector2(1920, 1080);
            canvasScaler2.matchWidthOrHeight = 0.5f;

            canvasObj2.AddComponent<GraphicRaycaster>();

            canvasObj2.transform.SetParent(_parent, false);
            canvas2.sortingLayerName = "UI";
            canvas2.sortingOrder = 13;
            
            // Instanțiere Player Health UI în același Canvas
            GameObject playerHealthUI = GameObject.Instantiate(playerHealthPrefab, canvasObj2.transform);

            // Ajustare Transform pentru poziționare corectă în UI
            RectTransform healthRectTransform = playerHealthUI.GetComponent<RectTransform>();
            if (healthRectTransform != null)
            {
                healthRectTransform.anchorMin = new Vector2(0f, 1f);
                healthRectTransform.anchorMax = new Vector2(0f, 1f);
                healthRectTransform.pivot = new Vector2(1f, 1f);
                healthRectTransform.anchoredPosition = new Vector2(1000, 270); 
            }

            PlayerHealth playerHealth=player.GetComponent<PlayerHealth>();
            playerHealth.healthBar=playerHealthUI.GetComponent<HealthBar>();
            
            
            

            GameObject joy = GameObject.Instantiate(joystick, canvasObj2.transform);
            RectTransform joyRectTransform = joy.GetComponent<RectTransform>();
            if (joyRectTransform != null)
            {
                joyRectTransform.anchoredPosition=new Vector2(196, 196);
            }

            PlayerController playerContr = player.GetComponent<PlayerController>();
            playerContr.canvasManager = this;
            playerContr._joystick = joy.GetComponent<FixedJoystick>();
            jumpsTotal = playerContr.maxJumps;
            int spacing = 48;
            for (int i = 0; i < jumpsTotal; i++)
            {
                GameObject jumpused = GameObject.Instantiate(jump_used, canvasObj2.transform);
                jumpused.name=$"jumpUsed {i}";
                
                GameObject jumpactive = GameObject.Instantiate(jump_active, canvasObj2.transform);
                jumpactive.name = $"jumpActive {i}";
                jumpActiveIcons.Add(jumpactive);
                RectTransform jumpUsed = jumpused.GetComponent<RectTransform>();
                RectTransform jumpActive = jumpactive.GetComponent<RectTransform>();
                jumpUsed.anchoredPosition = (new Vector2(-270-i*spacing, 37));
                jumpActive.anchoredPosition = (new Vector2(-270-i*spacing, 37));


            }

            // ===== Joystick Touch Area Full Screen =====                                              ASTAAAAAAAAAAAAAAAAAAAAAA
            /*GameObject touchArea = new GameObject("JoystickTouchArea");
            touchArea.transform.SetParent(canvasObj2.transform, false);

            RectTransform touchRect = touchArea.AddComponent<RectTransform>();
            touchRect.anchorMin = Vector2.zero;
            touchRect.anchorMax = Vector2.one;
            touchRect.offsetMin = Vector2.zero;
            touchRect.offsetMax = Vector2.zero;

            Image touchImage = touchArea.AddComponent<Image>();
            touchImage.color = new Color(0, 0, 0, 0); // invizibil
            touchImage.raycastTarget = true;*/

            JoystickController joystickController = joy.AddComponent<JoystickController>();
            joystickController.joystickImage = joyRectTransform;
            
            joy.SetActive(true);
            
            GameObject jumpButt = GameObject.Instantiate(jumpButton, canvasObj2.transform);
            RectTransform jumpButtRectTransform=jumpButt.GetComponent<RectTransform>();
            if (jumpButtRectTransform != null)
            {
                jumpButtRectTransform.anchoredPosition=new Vector2(-165, 75);
            }
            Button jumpBtnComponent = jumpButt.GetComponent<Button>();
            jumpBtnComponent.onClick.AddListener(() => playerContr.Jump());

            Glock glock = player.GetComponentInChildren<Glock>();
            /*for (int i = 0; i < glock.maxAmmo; i++)
            {
                GameObject ammoInstance = GameObject.Instantiate(AmmoOnScreen, canvasObj2.transform);
                ammoInstance.name = $"Ammo_{i}";
            }*/
            
            
            
        }
    }
}
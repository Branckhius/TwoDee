using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.Constants;
using Project.Scripts.DataCore.DataStructure;
using Project.Scripts.Game.Menu;
using Project.Scripts.Game.UI.SceneSystem;
using Project.Scripts.Messages.Requests.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Game.GameManager.States
{
    public class GameStateMenu : GameStateBase<GameStateMenu.Context>
    {
        public class Context { }
        
        private readonly LifetimeScope _currentScope;
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly ISceneSystem _sceneSystem;
        private IGameManager _gameManager;

        private TaskCompletionSource<string> _buttonPressedTCS = new(); // Task pentru butoane

        [Inject]
        public GameStateMenu(LifetimeScope currentScope, 
            IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, 
            ISceneSystem sceneSystem,
            IGameManager gameManager) // Injectăm GameManager
        {
            _currentScope = currentScope;
            _scopeCreator = scopeCreator;
            _sceneSystem = sceneSystem;
            _gameManager = gameManager; // Salvăm referința
            UniTask.DelayFrame(1);
        }

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            Debug.Log("Suntem in Menu");

            var scopeResult = _scopeCreator.Invoke(new CreateScopeRequest
            {
                childName = ScopeNames.MENU_SCOPE,
                parentScope = _currentScope,
            });

            var scope = scopeResult.childScope;
            Debug.Log(scope + " scope scope");

            var menuScope = scope as MenuScope;
            var menuConfig = menuScope.Container.Resolve<MenuScopeConfiguration>();
            var scene = _sceneSystem.GetScene();

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(scene.transform);
            EventSystem eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();

            GameObject cameraMenuObject = new GameObject("MenuCamera");
            Camera cameraMenu = cameraMenuObject.AddComponent<Camera>();
            cameraMenu.orthographic = true;
            cameraMenu.clearFlags = CameraClearFlags.SolidColor;
            cameraMenu.backgroundColor = Color.black;
            cameraMenu.orthographicSize = 9.6f;
            cameraMenu.transform.SetParent(scene.transform);

            GameObject canvasObject = new GameObject("UICanvas");
            Canvas uiCanvas = canvasObject.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            uiCanvas.worldCamera = cameraMenu;
            uiCanvas.planeDistance = 1f;
            if (uiCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                uiCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObject.transform.SetParent(cameraMenuObject.transform);
            canvasObject.transform.localPosition = Vector3.zero;
            canvasObject.transform.localScale = Vector3.one;

            GameObject bgObject = Object.Instantiate(menuConfig.backMenuScreen, canvasObject.transform);
            RectTransform bgRect = bgObject.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgRect.anchoredPosition = Vector3.zero;

            GameObject splashScreen = GameObject.Find("SplashScreen");
            if (splashScreen != null)
            {
                Canvas splashCanvas = splashScreen.GetComponent<Canvas>();
                if (splashCanvas != null)
                {
                    splashCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    splashCanvas.worldCamera = cameraMenu;
                    splashCanvas.planeDistance = 1f;
                }
            }

            Debug.Log("Menu UI configurat corect!");

            GameObject labelsParent = new GameObject("LabelsParent");
            labelsParent.transform.SetParent(canvasObject.transform);
            labelsParent.transform.localPosition = Vector3.zero;
            labelsParent.transform.localScale = Vector3.one;

            GameObject textParent = new GameObject("TextParent");
            textParent.transform.SetParent(canvasObject.transform);
            textParent.transform.localPosition = Vector3.zero;
            textParent.transform.localScale = Vector3.one;

            GameObject buttonsParent = new GameObject("ButtonsParent");
            buttonsParent.transform.SetParent(canvasObject.transform);
            buttonsParent.transform.localPosition = Vector3.zero;
            buttonsParent.transform.localScale = Vector3.one;
            
            Object.Instantiate(menuConfig.newGameText, new Vector3(0, 268, 0), Quaternion.identity, textParent.transform).transform.localScale = new Vector3(200, 200, 200);
            Object.Instantiate(menuConfig.continueGameText, new Vector3(187, 130, 0), Quaternion.identity, textParent.transform).transform.localScale=new Vector3(200,200,200);
            Object.Instantiate(menuConfig.inventoryText, new Vector3(0, -32, 0), Quaternion.identity, textParent.transform).transform.localScale=new Vector3(200,200,200);
            Object.Instantiate(menuConfig.optionsText, new Vector3(50, -254, 0), Quaternion.identity, textParent.transform).transform.localScale=new Vector3(200,200,200);
            Object.Instantiate(menuConfig.exitText, new Vector3(20, -573, 0), Quaternion.identity, textParent.transform).transform.localScale=new Vector3(200,200,200);

            CreateLabelWithButton(menuConfig.newGameLabel, new Vector3(0, 350, 0), new Vector3(140, 170, 1), () => OnButtonPressed("NewGame"));
            CreateLabelWithButton(menuConfig.continueGameLabel, new Vector3(0, 160, 0), new Vector3(140, 170, 1), () => OnButtonPressed("Continue"));
            CreateLabelWithButton(menuConfig.inventoryLabel, new Vector3(0, -30, 0), new Vector3(140, 170, 1), () => OnButtonPressed("Inventory"));
            CreateLabelWithButton(menuConfig.optionsLabel, new Vector3(0, -220, 0), new Vector3(140, 170, 1), () => OnButtonPressed("Options"));
            CreateLabelWithButton(menuConfig.exitLabel, new Vector3(0, -550, 0), new Vector3(100, 170, 1), () => OnButtonPressed("Exit"));

            Debug.Log("Așteptăm apăsarea unui buton...");

            // Așteptăm să fie apăsat un buton
            string buttonName = await _buttonPressedTCS.Task;

            Debug.Log($"Buton apăsat: {buttonName}");
            
            if (buttonName == "NewGame")
            {
                Object.Destroy(scene.gameObject);
                _buttonPressedTCS = new TaskCompletionSource<string>(); // Resetăm task-ul pentru butoane
                var gameplayContext = new GameStateGameplay.Context(new GameData());
                _gameManager.EnqueueSwitchState<GameStateGameplay, GameStateGameplay.Context>(gameplayContext);
            }
            else if (buttonName == "Exit")
            {
                Debug.Log("Ieșim din joc!");
                Application.Quit();
            }
            void CreateLabelWithButton(GameObject labelPrefab, Vector3 position, Vector3 scale, UnityEngine.Events.UnityAction action)
            {
                GameObject label = Object.Instantiate(labelPrefab, position, Quaternion.identity, labelsParent.transform);
                label.transform.localScale = scale;
                    
                GameObject button = new GameObject(labelPrefab.name + "Button");
                button.transform.SetParent(buttonsParent.transform);
                button.transform.position = position;
                button.transform.localScale = new Vector3(5.6f, 1.15f, 1);
                Button btnComponent = button.AddComponent<Button>();
                Image img = button.AddComponent<Image>();
                img.color = new Color(1, 1, 1, 0); // Transparent button
                    
                if (action != null)
                {
                    btnComponent.onClick.AddListener(action);
                }
            }
        }
        
        public void OnButtonPressed(string buttonName)
        {
            Debug.Log($"Buton apăsat: {buttonName}");

            // Setăm TaskCompletionSource cu numele butonului apăsat
            if (!_buttonPressedTCS.Task.IsCompleted)
            {
                _buttonPressedTCS.TrySetResult(buttonName);
            }
        }
        

        protected override void OnSuspend()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnResume()
        {
            throw new System.NotImplementedException();
        }
    }
}

    
    


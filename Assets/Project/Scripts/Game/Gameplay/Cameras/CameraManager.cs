using Cinemachine;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Cameras
{
    public class CameraManager
    {
        public Camera MainCamera { get; private set; }
        public Camera UICamera { get; private set; }
        public Camera GameCamera { get; private set; }
        public CinemachineVirtualCamera UIVirtualCamera { get; private set; }
        public CinemachineVirtualCamera GameVirtualCamera { get; private set; }

        public void InitializeCameras(Transform parent, GameObject player)
        {
            // Creare Main Camera
            GameObject mainCameraObj = new GameObject("Main Camera");
            MainCamera = mainCameraObj.AddComponent<Camera>();
            var brain = mainCameraObj.AddComponent<CinemachineBrain>();
            mainCameraObj.tag = "MainCamera";

            // Creare UI Camera
            GameObject uiCameraObj = new GameObject("UI Camera");
            UICamera = uiCameraObj.AddComponent<Camera>();
            UIVirtualCamera = uiCameraObj.AddComponent<CinemachineVirtualCamera>();
            UICamera.clearFlags = CameraClearFlags.Depth;

            // Creare Game Camera
            GameObject gameCameraObj = new GameObject("Game Camera");
            GameCamera = gameCameraObj.AddComponent<Camera>();
            GameVirtualCamera = gameCameraObj.AddComponent<CinemachineVirtualCamera>();
            GameCamera.clearFlags = CameraClearFlags.Skybox;

            // Setare transform
            mainCameraObj.transform.SetParent(parent);
            uiCameraObj.transform.SetParent(parent);
            gameCameraObj.transform.SetParent(parent);

            // Setare proiecție ortografică
            SetCameraProjection(MainCamera);
            SetCameraProjection(UICamera);
            SetCameraProjection(GameCamera);

            // Setare poziție pe Z pentru camere
            gameCameraObj.transform.position = new Vector3(0, 0, -5);
            uiCameraObj.transform.position = new Vector3(0, 0, 10);

            // Configurare UI Virtual Camera
            UIVirtualCamera.Priority = 10;
            UIVirtualCamera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
            UIVirtualCamera.m_Follow = player.transform;

            // Configurare Game Virtual Camera
            GameVirtualCamera.Priority = 9;
        }

        private void SetCameraProjection(Camera camera)
        {
            camera.orthographic = true;
            camera.orthographicSize = 10f;
        }

        public void ConfigureSplashScreen()
        {
            var splashScreen = GameObject.Find("SplashScreen");
            if (splashScreen != null)
            {
                var canvasSplash = splashScreen.GetComponent<Canvas>();
                if (canvasSplash != null)
                {
                    canvasSplash.renderMode = RenderMode.ScreenSpaceCamera;
                    canvasSplash.worldCamera = GameCamera;
                }
            }
        }
    }
}

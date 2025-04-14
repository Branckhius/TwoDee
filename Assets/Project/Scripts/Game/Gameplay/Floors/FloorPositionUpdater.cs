using System.Collections;
using Pathfinding;
using Project.Scripts.Game.Gameplay.Enemies;
using Project.Scripts.Game.UI.SceneSystem;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Floors
{
    public class FloorPositionUpdater : MonoBehaviour
    {
        public Camera _camera;
        public Transform _cameraTransform;
        public float _cameraHeight;
        private GameObject _floor;
        private GameObject _floor2;
        private GameObject _jumpPlace1;
        private GameObject _jumpPlace2;
        private GameObject _downPlace1;
        private GameObject _downPlace2;
        private GameObject AStar;
        private GameObject _Trick;
        public BaseScene Scene;
        public AstarPath PFController;
        private GridGraph grid;
        private Vector3 cameraCenter;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
    
        public GridManager _gridManager;
        private FloorManager _floorManager;
    

        public void Initialize(GameplayScopeConfiguration gameplayConfig, BaseScene scene, Camera camera)
        {
            _camera = camera;
            _cameraTransform = camera.transform;
            _cameraHeight = camera.orthographicSize * 2f;

            _floorManager = new FloorManager(scene.transform, gameplayConfig);
            _gridManager = new GridManager(scene.transform);

            _floor = gameObject;
            _floor.transform.localScale = new Vector3(_floor.transform.localScale.x / 8f, _floor.transform.localScale.y * 7.5f, 1f);
            BoxCollider2D floorCollider = _floor.GetComponent<BoxCollider2D>();
            floorCollider.size = new Vector2(4.79f, 0.12f);
            floorCollider.offset = Vector2.zero;
        
            _floor2 = _floorManager.CreateFloor(gameplayConfig.Floor2, "Ground");
            _floor2.transform.localScale = new Vector3(_floor2.transform.localScale.x, _floor2.transform.localScale.y * 4.2f * 1.212f, _floor2.transform.localScale.z);


            _jumpPlace1 = _floorManager.CreateFloor(gameplayConfig.JumpPlace, "JumpGround");
        
            _downPlace1 = _floorManager.CreateFloor(gameplayConfig.DownPlace, "DownGround");
            _downPlace2 = _floorManager.CreateFloor(gameplayConfig.DownPlace, "DownGround");
        
            _Trick = _floorManager.CreateFloor(gameplayConfig.Trick, "Trick");

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            StartCoroutine(DelayedFloorUpdate());
        }

        void Update()
        {
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                Debug.Log($"Rezoluția s-a schimbat la: {Screen.width}x{Screen.height}");
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;

                StartCoroutine(DelayedFloorUpdate());


            }
        }

        void UpdateFloor()
        {
            cameraCenter = _cameraTransform.position;
            Update1stPlatform();
            UpdateJumpPlaceFor1stPlatform(Screen.width * 0.26f * (_camera.orthographicSize * 2 / Screen.height),Screen.height * 0.39f * (_camera.orthographicSize * 2 / Screen.height),_jumpPlace1);
            UpdateDownPlacesFor1stPlatform(Screen.width * 0.46f * (_camera.orthographicSize * 2 / Screen.height),Screen.width * 0.06f * (_camera.orthographicSize * 2 / Screen.height),Screen.height * 0.16f * (_camera.orthographicSize * 2 / Screen.height),_downPlace1,_downPlace2);
        
            //Suprafata
            Vector3 cameraPosition = _cameraTransform.position;
            float adjustedY = cameraPosition.y - (_cameraHeight / 2) - 1f;
            adjustedY *= 0.85f;
        
            float offsetX = Screen.width * 0.132f * (_camera.orthographicSize * 2 / Screen.height);
            float offsetY = Screen.height * 0.219f * (_camera.orthographicSize * 2 / Screen.height);
        
            _Trick.transform.position = new Vector3(cameraCenter.x - offsetX, cameraCenter.y - offsetY, 5);
            BoxCollider2D trickCollider = _Trick.GetComponent<BoxCollider2D>();
            trickCollider.isTrigger = false;
        
            _floor.transform.position = new Vector3(cameraPosition.x, adjustedY, 5);

            float screenWidthUnits = _camera.orthographicSize * (Screen.width / (float)Screen.height) * 0.42f;
            float floorHeight = _floor.transform.localScale.y;
            _floor.transform.localScale = new Vector3(screenWidthUnits, floorHeight, 1f);
            
            _gridManager.UpdateGrid(cameraPosition,_camera);
            trickCollider.isTrigger = true;
        }
        
        private IEnumerator DelayedFloorUpdate()
        {
            yield return new WaitForEndOfFrame(); // Sau yield return null;
            UpdateFloor();
        }
        
        void Update1stPlatform()
        {
            float offsetX = Screen.width * 0.2615f * (_camera.orthographicSize * 2 / Screen.height);
            float offsetY = Screen.height * 0.219f * (_camera.orthographicSize * 2 / Screen.height);

            _floor2.transform.position = new Vector3(cameraCenter.x - offsetX, cameraCenter.y - offsetY, 5);
            float screenWidthUnits = _camera.orthographicSize * (Screen.width / (float)Screen.height) * 0.383f;
            float floor2Height = _floor2.transform.localScale.y;

            _floor2.transform.localScale = new Vector3(screenWidthUnits, floor2Height, 1f);
        }
    
        void UpdateDownPlacesFor1stPlatform(float offsetx1, float offsetx2,float offsety,GameObject obj1,GameObject obj2)
        {
            float offsetX1 = offsetx1;
            float offsetX2 = offsetx2;
            float offsetY = offsety;
            obj1.transform.position = new Vector3(cameraCenter.x - offsetX1, cameraCenter.y - offsetY, 5);
            obj2.transform.position = new Vector3(cameraCenter.x - offsetX2, cameraCenter.y - offsetY, 5);
        }
        
        void UpdateJumpPlaceFor1stPlatform(float offsetx1,float offsety,GameObject obj1)
        {
            float offsetX1 = offsetx1;
            float offsetY = offsety;
            obj1.transform.position = new Vector3(cameraCenter.x - offsetX1, cameraCenter.y - offsetY, 5);
        }

    }
}

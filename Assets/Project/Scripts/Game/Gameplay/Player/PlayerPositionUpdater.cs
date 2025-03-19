using Project.Scripts.Game.Gameplay.Enemies;
using Project.Scripts.Game.UI.SceneSystem;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class PlayerPositionUpdater : MonoBehaviour, IPlayerPositionUpdater
    {
        private Camera _camera;
        private Transform _cameraTransform;
        private float _cameraHeight;
        private GameObject _player;
        private GameObject _enemy;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private GameplayScopeConfiguration _gameplayConfig;
        private BaseScene _scene;

        public void Initialize(GameplayScopeConfiguration gameplayConfig, BaseScene scene, Camera gameCamera)
        {
            _gameplayConfig = gameplayConfig;
            _scene = scene;
            _camera = gameCamera;
            _cameraTransform = gameCamera.transform;
            _cameraHeight = gameCamera.orthographicSize * 2f;

            SetupPlayer();
            SetupEnemy();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            UpdatePlayerPosition();
        }

        private void SetupPlayer()
        {
            _player = gameObject;
            _player.transform.localScale = new Vector3(_player.transform.localScale.x * -10f,
                _player.transform.localScale.y * 0.320339f, 1f);
            _player.layer = LayerMask.NameToLayer("Player");
            BoxCollider2D collider = _player.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(7.1f, 6.21f);
            collider.offset = new Vector2(-0.03f, 4.365f);

        }

        private void SetupEnemy()
        {
            _enemy = Object.Instantiate(_gameplayConfig.Enemy, _scene.transform);
            _enemy.layer = LayerMask.NameToLayer("Enemy");
            _enemy.tag = "Enemy";
            BoxCollider2D collider = _enemy.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(0.1345796f, 0.1563134f);
            collider.offset = new Vector2(0f, 0f);
        
            EnemyAI enemyAI = _enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.target = _player.transform;
            }
        }



        private void Update()
        {
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                Debug.Log($"Rezoluția s-a schimbat la: {Screen.width}x{Screen.height}");
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                UpdatePlayerPosition();
            }
        }

        public void UpdatePlayerPosition()
        {
            Vector3 cameraPosition = _cameraTransform.position;
            float adjustedY = cameraPosition.y - (_cameraHeight / 2) + 1.5f;

            float screenOffset = Screen.width * 0.9f;
            Vector3 worldOffset = _camera.ScreenToWorldPoint(new Vector3(screenOffset, 0, _camera.nearClipPlane));
            _player.transform.position = new Vector3(cameraPosition.x - (worldOffset.x - cameraPosition.x), adjustedY, 0);

            float screenWidthUnits = _camera.orthographicSize * (Screen.width / (float)Screen.height) * 0.018f;
            _player.transform.localScale = new Vector3(screenWidthUnits, screenWidthUnits, 1f);

            float adjustedY2 = cameraPosition.y - (_cameraHeight / 2) + 2f;
            float screenOffset2 = Screen.width * 0.1f;
            Vector3 worldOffset2 = _camera.ScreenToWorldPoint(new Vector3(screenOffset2, 0, _camera.nearClipPlane));
            _enemy.transform.position = new Vector3(cameraPosition.x - (worldOffset2.x - cameraPosition.x), adjustedY2, 0);

            _enemy.transform.localScale = new Vector3(screenWidthUnits*40f, screenWidthUnits*40f, 1f);
        }
        
    }
}

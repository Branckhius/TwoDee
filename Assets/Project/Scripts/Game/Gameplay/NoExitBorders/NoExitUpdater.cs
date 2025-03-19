using Project.Scripts.Game.UI.SceneSystem;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.NoExitBorders
{
    public class NoExitUpdater : MonoBehaviour
    {
        public Camera camera;
        public Transform cameraTransform;
        public float cameraHeight;
        private GameObject floor1;
        private GameObject floor2;
        private GameObject floor3;

        private int lastScreenWidth;
        private int lastScreenHeight;
        public GameplayScopeConfiguration GameplayConfig;
        public BaseScene Scene;

        void Start()
        {
            floor1 = gameObject;
            floor2 = Object.Instantiate(GameplayConfig.Floor, Scene.transform);
            floor3 = Object.Instantiate(GameplayConfig.Floor, Scene.transform);
        
            GameObject Bariere=new GameObject();
            Bariere.name="Bariere";
            Bariere.transform.SetParent(Scene.transform);
            floor1.name = "barieraStanga";
            floor1.transform.SetParent(Bariere.transform);
            floor2.name = "barieraSus";
            floor2.transform.SetParent(Bariere.transform);
            floor3.name = "barieraDreapta";
            floor3.transform.SetParent(Bariere.transform);

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        
            floor1.transform.rotation = Quaternion.Euler(0, 0, 90);
            floor2.transform.rotation = Quaternion.Euler(0, 0, 180);
            floor3.transform.rotation = Quaternion.Euler(0, 0, 270);
            AddCollider(floor1);
            AddCollider(floor2);
            AddCollider(floor3);
        
            UpdateFloor();
        }

        void Update()
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                UpdateFloor();
            }
        }
    
        void UpdateFloor()
        {
            Vector3 cameraPosition = cameraTransform.position;
            float screenWidthUnits = camera.orthographicSize * (Screen.width / (float)Screen.height) * 2;
            float screenHeightUnits = camera.orthographicSize * 2;
        
            // Floor Left & Right - Acoperă înălțimea ecranului
            float floorVerticalLength = screenHeightUnits * 1.5f; // Scalare cu 50% mai mare
            float floorVerticalHeight = floor1.transform.localScale.y;
        
            // Floor Top - Acoperă lățimea ecranului
            float floorHorizontalLength = screenWidthUnits * 1.5f; // Scalare cu 50% mai mare
            float floorHorizontalHeight = floor2.transform.localScale.y;
        
            // Aplică scalarea
            floor1.transform.localScale = new Vector3(floorVerticalLength/7.199f, floorVerticalHeight, 1f);
            floor2.transform.localScale = new Vector3(floorHorizontalLength/7.159f, floorHorizontalHeight, 1f);
            floor3.transform.localScale = new Vector3(floorVerticalLength/7.199f, floorVerticalHeight, 1f);
        
            // Floor Left (rotit 90 grade)
            floor1.transform.position = new Vector3((cameraPosition.x - screenWidthUnits / 2) - 0.1f, cameraPosition.y, 0);
        
            // Floor Top (rotit 180 grade)
            floor2.transform.position = new Vector3(cameraPosition.x, (cameraPosition.y + screenHeightUnits / 2) + 0.2f, 0);
        
            // Floor Right (rotit 270 grade)
            floor3.transform.position = new Vector3((cameraPosition.x + screenWidthUnits / 2) + 0.1f, cameraPosition.y, 0);
        }
    
        void AddCollider(GameObject floor)
        {
            BoxCollider2D floorCollider = floor.GetComponent<BoxCollider2D>();
            if (floorCollider == null)
                floorCollider = floor.AddComponent<BoxCollider2D>();
        
            floorCollider.size = new Vector2(4.79f, 0.12f);
            floorCollider.offset = Vector2.zero;
        }
    }
}

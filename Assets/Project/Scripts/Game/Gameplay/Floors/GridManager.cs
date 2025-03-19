using Pathfinding;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Floors
{
    public class GridManager
    {
        private AstarPath _pfController;
        private GridGraph _grid;
        private GameObject _astarObject;
    
        public GridManager(Transform parent)
        {
            _astarObject = new GameObject("A*");
            _astarObject.transform.SetParent(parent);

            _pfController = _astarObject.AddComponent<AstarPath>();
            AstarData data = _pfController.data;
            _grid = data.AddGraph(typeof(GridGraph)) as GridGraph;
        
            _grid.SetDimensions(36, 20, 0.98f);
            _grid.is2D = true;
            _grid.collision.mask = LayerMask.GetMask("Ground");
            _grid.collision.use2D = true;
        }

        public void UpdateGrid(Vector3 cameraPosition, Camera camera)
        {
            Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        
            float gridWidth = topRight.x - bottomLeft.x;
            float gridHeight = topRight.y - bottomLeft.y;
            int widthNodes = Mathf.RoundToInt(gridWidth);
            int heightNodes = Mathf.RoundToInt(gridHeight);

            _grid.SetDimensions(widthNodes, heightNodes, 0.98f);
            _pfController.Scan();

            _astarObject.transform.position = new Vector3(Mathf.Abs(cameraPosition.x), cameraPosition.y, 0);
        }
    }
}
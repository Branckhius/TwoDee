using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform joystickImage;
        private Vector3 initialPosition;
        private Canvas canvas;
        
        public

        void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            initialPosition = new Vector2(196, 196);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 worldPos = canvas.worldCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            joystickImage.position = worldPos;
            //joystickImage.position = new Vector2(0, -7.5f);

            // Forțează joystick-ul să-și updateze centrul
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joystickImage.anchoredPosition = initialPosition;
        }
    }
}
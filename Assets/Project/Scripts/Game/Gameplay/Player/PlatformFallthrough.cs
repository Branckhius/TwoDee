using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class PlatformFallthrough : MonoBehaviour
    {
        public Collider2D physicalCollider; // Coliderul standard (ne-trigger)
        private void OnTriggerEnter2D(Collider2D other)
        {
            /*Debug.Log("A intrat in enterpre");

            if (other.CompareTag("Player"))
            {
                Debug.Log("A intrat in enter");

                // Ignoră coliziunea cât timp playerul este în trigger
                Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), physicalCollider, true);
            }*/
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            /*Debug.Log("A intrat in exitprez");

            if (other.CompareTag("Player"))
            {
                Debug.Log("A intrat in exit");
                // Repornește coliziunea când iese din trigger
                Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), physicalCollider, false);
            }*/
        }
    }
}
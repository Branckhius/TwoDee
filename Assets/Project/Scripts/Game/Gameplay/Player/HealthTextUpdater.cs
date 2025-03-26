using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class HealthTextUpdater : MonoBehaviour
    {
        public Slider healthBar;
        public Image[] numberImages; // Array cu imaginea fiecărei cifre
        public Sprite[] digitSprites; // Array cu sprites pentru cifre (0-9)

        void Update()
        {
            UpdateHealthText((int)healthBar.value);
        }

        void UpdateHealthText(int value)
        {
            string text = value.ToString(); // Convertim valoarea în string

            // Ascunde toate imaginile inițial
            foreach (var img in numberImages)
                img.gameObject.SetActive(false);

            // Setează sprite-urile corecte pentru fiecare cifră
            for (int i = 0; i < text.Length; i++)
            {
                int digit = text[i] - '0'; // Convertim caracterul în număr
                numberImages[i].sprite = digitSprites[digit];
                numberImages[i].gameObject.SetActive(true);
            }
        }
    }
}
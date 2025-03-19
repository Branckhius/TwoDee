using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 20f;
        public LayerMask groundLayer;
    
        private Rigidbody2D rb;
        private Animator animator;
        private int jumpCount = 0;
        private int maxJumps = 1;
        private bool isGrounded;
        private bool isFacingRight = false;

        // Câmpul pentru arma curentă
        private Weapon currentWeapon;
        public GameplayScopeConfiguration _gameplayConfig;
        public Camera _camera;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            rb.gravityScale = 2f;
            rb.freezeRotation = true;
            groundLayer = LayerMask.GetMask("Ground");
            GameObject glockGO = Instantiate(_gameplayConfig.Glock, transform.position, transform.rotation);

            // Obține componenta Weapon (Glock) de pe obiectul instanțiat
            Weapon glockWeapon = glockGO.GetComponent<Weapon>();
            Glock glockStats=glockGO.GetComponent<Glock>();
            glockStats.gameplayConfig = _gameplayConfig;
            SetWeapon(glockWeapon);
        }

        void Update()
        {
            float moveX = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    
            // Schimbă direcția doar când player-ul se mișcă
            if (moveX > 0 && isFacingRight) 
            {
                transform.eulerAngles = new Vector3(0, -180f, 0);
                isFacingRight = false;
            } 
            else if (moveX < 0 && !isFacingRight) 
            {
                transform.eulerAngles = new Vector3(0, 0f, 0);
                isFacingRight = true;
            }

            // Verifică dacă e pe sol
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
            Debug.DrawRay(transform.position, Vector2.down * 1.2f, hit.collider ? Color.green : Color.red);
            isGrounded = hit.collider != null;

            if (isGrounded)
            {
                jumpCount = 0; // Resetează săriturile
            }

            // Săritura și double jump
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
            }

            // Setează parametrii în Animator
            animator.SetBool("Running", Mathf.Abs(moveX) > 0.1f);
            animator.SetFloat("VerticalVelocity", rb.velocity.y); // Pentru diferențierea între Jump și Fall
            animator.SetBool("isGrounded", isGrounded);

            // Animațiile de Jump în funcție de viteza verticală
            if (isGrounded)
            {
                animator.SetBool("Jump", false); // Player-ul este pe sol
            }
            else if (rb.velocity.y > 0)
            {
                animator.SetBool("Jump", true); // Player-ul este în sus (sare)
            }
            else if (rb.velocity.y < 0)
            {
                animator.SetBool("Jump", false); // Player-ul coboară
            }
            
        }
        
        public void SetWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
    
            // Atașăm arma la player
            weapon.transform.SetParent(transform);
    
            // Poziționăm arma relativ la player
            PositionWeapon();
        }

        private void PositionWeapon()
        {
            if (currentWeapon == null) return;

            // Folosim dimensiunea ecranului pentru a ajusta poziția Glock-ului
            float screenOffset = Screen.width * 0.12f; // Poziționează puțin mai la dreapta
            Vector3 worldOffset = _camera.ScreenToWorldPoint(new Vector3(screenOffset, 0, _camera.nearClipPlane));

            // Setăm poziția relativ la player
            currentWeapon.transform.position = new Vector3(
                transform.position.x + (worldOffset.x - transform.position.x)-0.2f,
                transform.position.y+0.9f,
                0
            );

            // Ajustăm mărimea armei în funcție de player
            float screenWidthUnits = _camera.orthographicSize * (Screen.width / (float)Screen.height) * 0.42f;
            currentWeapon.transform.localScale = new Vector3(screenWidthUnits, screenWidthUnits, 1f);
        }

    }
}


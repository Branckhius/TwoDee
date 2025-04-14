using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Game.Gameplay.CanvasBG;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 7f;
        public float jumpForce = 20f;
        public LayerMask groundLayer;
    
        private Rigidbody2D rb;
        private Animator animator;
        private int jumpCount = 0;
        public int maxJumps = 2;
        private bool isGrounded;
        private bool isFacingRight = false;

        // Câmpul pentru arma curentă
        private Weapon currentWeapon;
        public GameplayScopeConfiguration _gameplayConfig;
        public Camera _camera;
        public CanvasManager canvasManager;

        public FixedJoystick _joystick;
        private Collider2D playerCollider;
        private Collider2D platformCollider;
        private bool isOnPlatform = false;
        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerCollider = GetComponent<Collider2D>();
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

        void Update()//nu schimba fixedupdate, numai daca faci un update special pentru joystick
        {
            float moveX = _joystick.Horizontal;
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
            Vector2 size = GetComponent<Collider2D>().bounds.size;

// Start point: colțul dreapta-jos, ridicat puțin mai mult ca să nu fie prea jos
            Vector2 bottomRight = (Vector2)transform.position + new Vector2(size.x / 2f, -size.y / 2f + 1);

// Direcția orizontală spre stânga
            Vector2 direction = Vector2.left;
            float rayDistance = size.x;

            RaycastHit2D hit = Physics2D.Raycast(bottomRight, direction, rayDistance, groundLayer);
            Debug.DrawRay(bottomRight, direction * rayDistance, hit.collider ? Color.green : Color.red);



            isGrounded = hit.collider != null;
            
            Debug.Log(_joystick.Vertical);

            if (hit.collider != null) 
            {
                // Verificăm dacă obiectul detectat are tag-ul "Platform"
                if (hit.collider.CompareTag("Platform"))
                {
                    PlatformEffector2D effector = hit.collider.GetComponent<PlatformEffector2D>();

                    // Setăm platforma ca fiind sub jucător
                    platformCollider = hit.collider;
                    Collider2D col = hit.collider.GetComponent<BoxCollider2D>();
                    isGrounded = true;
                    // Dacă joystick-ul este apăsat în jos, ignoră coliziunea cu platforma
                    if (_joystick.Vertical < -0.47)  // Joystick-ul în jos
                    {
                        //hit.collider.isTrigger = true;
                        //effector.surfaceArc = 0f;
                        //Physics2D.IgnoreCollision(playerCollider, hit.collider, true);
                        TemporarilyIgnoreCollision(playerCollider,col).Forget();;
                    }
                    else
                    {
                        

                    }

                    /*if (hit.collider.CompareTag("Ground"))
                    {
                        effector.surfaceArc = 157.53f;

                    }*/

                }
            }
            else
            {

                isGrounded = false;
                platformCollider = null; // Dacă nu mai este pe sol, nu mai avem platformă
            }
            if (isGrounded && rb.velocity.y==0)
            {
                jumpCount = 0;   // Resetează săriturile
                UpdateJumpIcons();
            }

            // Săritura și double jump
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
            {
                Jump();
            }

            // Setează parametrii în Animator
            animator.SetBool("Running", Mathf.Abs(moveX) > 0.1f);
            animator.SetFloat("VerticalVelocity", rb.velocity.y); // Pentru diferențierea între Jump și Fall
            animator.SetBool("isGrounded", isGrounded);

            // Animațiile de Jump în funcție de viteza verticală
            if (isGrounded && rb.velocity.y==0)
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
        public void Jump()
        {
            if (jumpCount < maxJumps)//pus if aici iar pentru joystick, iar mai sus e pentru keyboard
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                
                UpdateJumpIcons();
            }
        }
        
        private async UniTask TemporarilyIgnoreCollision(Collider2D playerCollider,Collider2D platformColliderr)
        {
            Debug.Log("yaaa1");
            if (playerCollider == null) return;
            Debug.Log("yaaa2");

            Collider2D platformCollider = GetComponent<Collider2D>();
            if (platformCollider == null) return;
            Debug.Log("yaaa3");

            Physics2D.IgnoreCollision(playerCollider, platformColliderr, true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            Physics2D.IgnoreCollision(playerCollider, platformColliderr, false);
            Debug.Log("yaaa4");

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
        
        private void UpdateJumpIcons()
        {
            for (int i = 0; i < canvasManager.jumpActiveIcons.Count; i++)
            {
                if (i < maxJumps - jumpCount)
                {
                    canvasManager.jumpActiveIcons[i].SetActive(true);
                }
                else
                {
                    canvasManager.jumpActiveIcons[i].SetActive(false);
                }
            }
        }

        

    }
}


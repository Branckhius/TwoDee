using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Player
{
    public class Glock : Weapon
    {
        [Header("Glock Settings")]
        public GameObject bulletPrefab; // Prefab-ul glonțului
        public float bulletSpeed = 20f; // Viteza glonțului
        public Animator animator;       // Animatorul armei
        private bool canShoot = true;
        private int shotsFired = 0;     // Contor pentru numărul de focuri
        private const int maxShots = 7; // Numărul maxim de focuri înainte de reload
        public Transform firePoint; // Locul de unde iese glonțul
        public GameplayScopeConfiguration gameplayConfig;
        private GameObject shotsShot;
        private GameObject gameplayScope;

        void Start()
        {
            animator = GetComponent<Animator>();
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform); // Face parte din Glock
            firePoint.localPosition = new Vector3(0.21f, 0.138f, 0);
            shotsShot = new GameObject("ShotsShot");
            gameplayScope = GameObject.Find("Gameplay(Clone)"); 
            shotsShot.transform.SetParent(gameplayScope.transform);
        }

        void Update()
        {
            LookAtClosestEnemy();
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null && canShoot)
            {
                Fire().Forget(); // Lansează UniTask fără să blocheze jocul
            }
        }

        // Suprascriem metoda Fire()
        public override async UniTask Fire()
        {
            if (!canShoot) return;
            canShoot = false;

            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            // Instanțiază glonțul
            GameObject bullet = Instantiate(gameplayConfig.GlockBullet, firePoint.position, Quaternion.identity,gameObject.transform);
            bullet.transform.SetParent(shotsShot.transform);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Găsește cel mai apropiat inamic
                GameObject target = FindClosestEnemy();
                if (target != null)
                {
                    // Setează direcția glonțului către inamic
                    Vector2 direction = (target.transform.position - firePoint.position).normalized;
                    rb.velocity = direction * bulletSpeed;
                }
                else
                {
                    // Dacă nu există inamic, trage drept
                    rb.velocity = firePoint.right * bulletSpeed;
                }
            }

            shotsFired++;

            // Așteaptă 10% mai puțin timp înainte de a putea trage din nou
            await UniTask.Delay(500);

            if (shotsFired >= maxShots)
            {
                await Reload();
            }
            else
            {
                canShoot = true;
            }
        }


        // Suprascriem metoda Reload()
        public override async UniTask Reload()
        {
            if (animator != null)
            {
                animator.SetTrigger("Reload");
            }

            // Așteaptă 1.5 secunde pentru animația de reload
            await UniTask.Delay(1500);

            // Resetăm contorul de gloanțe și permitem tragerea
            shotsFired = 0;
            canShoot = true;
        }

        void LookAtClosestEnemy()
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Collider2D enemyCollider = closestEnemy.GetComponent<Collider2D>();
                if (enemyCollider != null)
                {
                    Vector3 enemyCenter = enemyCollider.bounds.center;
                    Vector3 direction = enemyCenter - transform.position;
                    float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    if (angleZ > 90f || angleZ < -90f)
                    {
                        transform.rotation = Quaternion.Euler(0, 180, -(angleZ - 180f));
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(0, 0, angleZ);
                    }
                }
            }
        }

        GameObject FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float minDistance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistance)
                {
                    closest = enemy;
                    minDistance = distance;
                }
            }
            return closest;
        }
    }
}

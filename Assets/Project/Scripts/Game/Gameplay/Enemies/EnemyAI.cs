    using System.Collections.Generic;
    using Pathfinding;
    using UnityEngine;

    namespace Project.Scripts.Game.Gameplay.Enemies
    {
    public class EnemyAI : MonoBehaviour
    {
        [Header("Pathfinding")]
        public Transform target;
        private Transform ReplacedTarget;
        public float activateDistance = 50f;
        public float pathUpdateSeconds = 0.5f;

        private BoxCollider2D col;

        [Header("Physics")]
        public float speed = 76f;
        public float nextWaypointDistance = 3f;
        public float jumpNodeHeightRequirement = 0.8f;
        public float jumpModifier = 0.3f;
        public float jumpCheckOffset = 0.1f;

        [Header("Custom Behavior")]
        public bool followEnabled = true;
        public bool jumpEnabled = true;
        public bool directionLookEnabled = true;

        [SerializeField] public LayerMask jumpableGround;

        private Path path;
        private int currentWaypoint = 0;
        private Animator anim;

        Seeker seeker;
        Rigidbody2D rb;
        private Rigidbody2D targetrb;

        private bool isJumpingToPlatform = false;
        private GameObject[] allObjects;
        private Transform Player;

        private List<Transform> jumpGrounds = new List<Transform>();
        
        void Start()
        {
            targetrb = target.GetComponent<Rigidbody2D>();
            ReplacedTarget = target;
            allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.layer == LayerMask.NameToLayer("JumpGround"))
                {
                    jumpGrounds.Add(obj.transform);
                }
            }

            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void Awake()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<BoxCollider2D>();
            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
            anim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (TargetInDistance() && followEnabled)
            {
                if (isJumpingToPlatform && Mathf.Abs(transform.position.y - ReplacedTarget.position.y) < 0.5f)
                {
                    target = ReplacedTarget;
                    isJumpingToPlatform = false;
                    UpdatePath();
                }

                PathFollow();
            }
        }

        private void UpdatePath()
        {
            jumpableGround = LayerMask.GetMask("Ground");
            if (followEnabled && TargetInDistance() && seeker.IsDone())
            {
                Transform currentTarget = isJumpingToPlatform ? target : ReplacedTarget;
                BoxCollider2D targetCollider = currentTarget.GetComponent<BoxCollider2D>();
                Vector2 adjustedTarget = new Vector2(
                    currentTarget.position.x, 
                    currentTarget.position.y + targetCollider.bounds.extents.y
                );
                Vector3 enemyStartPosition = col.bounds.center + new Vector3(0, -col.bounds.size.y * 0.23f, 0);
                seeker.StartPath(enemyStartPosition, adjustedTarget, OnPathComplete);
            }
        }

        private void PathFollow()
        {
            if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }
            
            RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.05f, jumpableGround);
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            // Mișcare - Apelăm trigger-ul pentru mișcare
            if (rb.velocity.x != 0)
            {
                anim.SetTrigger("Move"); // Asigură-te că există un trigger "Move" în animator
            }

            // Salt - Apelăm trigger-ul pentru săritură
            if (jumpEnabled && isGrounded)
            {
                targetrb = target.GetComponent<Rigidbody2D>();
                if (target.position.y -2f > rb.transform.position.y)// && targetrb.velocity.y == 0 && path.path.Count < 20)
                {
                    Transform closestJumpGround = FindClosestJumpGround();
                    Debug.Log($"Schimb target-ul la: {closestJumpGround.name}");
                    target = closestJumpGround;
                    isJumpingToPlatform = true;
                }
                else if (target.position.y + 2f < rb.transform.position.y && targetrb.velocity.y == 0 && path.path.Count < 20)
                {
                    Transform closestJumpGround = FindClosestDownGround();
                    target = closestJumpGround;
                    isJumpingToPlatform = true;
                }
            }

            rb.AddForce(Vector2.right * direction, ForceMode2D.Impulse);

            float maxSpeed = speed * 0.3f; // Ajustează procentul în funcție de cât de lent vrei să fie

            rb.velocity = new Vector2(
                Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
                rb.velocity.y
            );

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance * 0.6f)
            {
                currentWaypoint++;
            }

            if (directionLookEnabled)
            {
                if (rb.velocity.x > 0.05f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (rb.velocity.x < -0.05f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
        }

        private bool TargetInDistance()
        {
            return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        private Transform FindClosestJumpGround()
        {
            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach (Transform obj in jumpGrounds)
            {
                float distance = Vector2.Distance(obj.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = obj;
                }
            }

            return closest;
        }

        private Transform FindClosestDownGround()
        {
            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach (GameObject obj in allObjects)
            {
                if (obj.layer == LayerMask.NameToLayer("DownGround"))
                {
                    float distance = Vector2.Distance(obj.transform.position, target.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = obj.transform;
                    }
                }
            }

            return closest;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("JumpGround"))
            {
                
                if (transform.position.y +2f < Player.position.y)
                {
                    RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.05f, jumpableGround);
                    if (isGrounded)
                    {
                        rb.AddForce(Vector2.up * speed * jumpModifier * 485f);
                    }
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("DownGround"))
            {
                Transform closestDownGround = FindClosestDownGround();
                if (closestDownGround != null)
                {
                    target = closestDownGround;
                    isJumpingToPlatform = true;
                    UpdatePath();
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("JumpGround") && transform.position.y + 2f < Player.position.y)
            {
                RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.05f, jumpableGround);
                if (isGrounded)
                {
                    rb.AddForce(Vector2.up * speed * jumpModifier * 485f);

                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("JumpGround") || other.gameObject.layer == LayerMask.NameToLayer("DownGround"))
            {

                target = Player;
                isJumpingToPlatform = false;
                UpdatePath();
            }
        }
    }

    }
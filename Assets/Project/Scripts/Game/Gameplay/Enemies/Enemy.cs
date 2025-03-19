    namespace Project.Scripts.Game.Gameplay.Enemies
{
    using UnityEngine;

    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Settings")]
        public int maxHealth = 250;
        private Health health;
        public float attackRange = 2.5f;
        public int attackDamage = 10;
        public float attackCooldown = 1.5f;
        private bool canAttack = true;
        private bool isDead = false;
        
        private Animator animator;
        private Transform player;
        private EnemyAI enemyAI;

        void Start()
        {
            // Initializarea sănătății folosind clasa Health
            health = new Health(maxHealth);
            animator = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            enemyAI = GetComponent<EnemyAI>();
        }

void Update()
{
    if (player != null)
    {
        // Verifică distanța dintre inamic și jucător
        float distance = Vector2.Distance(transform.position, player.position);

        // Logica pentru a face inamicul să se uite către jucător
        if (player.position.x < transform.position.x)
        {
            if (transform.localScale.x > 0) // Dacă inamicul se uită deja la stânga, nu face nimic
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (transform.localScale.x < 0) // Dacă inamicul se uită deja la dreapta, nu face nimic
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        // Dacă este în intervalul de atac și poate ataca, execută atacul
        if (distance <= attackRange && canAttack)
        {
            Attack();
        }
    }
}


        public void TakeDamage(int damage)
        {
            if (isDead) return;

            health.Reduce(damage);
            Debug.Log($"Enemy took {damage} damage. Current HP: {health.CurrentHealth}");

            // Oprește mișcarea inamicului
            if (enemyAI != null)
            {
                enemyAI.followEnabled = false; // Oprește urmărirea
                Invoke(nameof(ResumeMovement), 0.23f); // Repornește după 1 secundă
            }

            if (health.IsDead)
            {
                Die();
            }
            else
            {
                animator.SetTrigger("Hurt");
            }
        }
        private void ResumeMovement()
        {
            if (!isDead && enemyAI != null)
            {
                enemyAI.followEnabled = true;
            }
        }

        private void Attack()
        {
            canAttack = false;

            float randomAttack = Random.Range(0, 2.5f);
            if (randomAttack == 0)
                animator.SetTrigger("Attack1");
            else
                animator.SetTrigger("Attack2");

            Invoke(nameof(DealDamageToPlayer), 0.5f);
            Invoke(nameof(ResetAttack), attackCooldown);
        }

        private void DealDamageToPlayer()
        {
            if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                IDamageable playerHealth = player.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        private void ResetAttack()
        {
            canAttack = true;
        }

        private void Die()
        {
            if (isDead) return; // Evită multiple apeluri
            isDead = true; // Setează flag-ul

            gameObject.layer = LayerMask.NameToLayer("Default");
            animator.SetTrigger("Death");
            
            GetComponent<EnemyAI>().enabled = false;

            Destroy(gameObject, 1.5f); // Distruge inamicul după animație
        }
    }
}

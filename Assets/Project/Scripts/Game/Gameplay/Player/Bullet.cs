using Project.Scripts.Game.Gameplay.Enemies;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 100;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) // Verifică layer-ul
        {
            Destroy(gameObject);
        }

    }

}
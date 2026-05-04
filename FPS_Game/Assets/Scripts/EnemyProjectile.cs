using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        // Si toca al jugador
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Se destruye al tocarte
        }
        // Si toca el suelo o paredes
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}

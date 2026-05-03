using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    private bool isDead = false;

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // Detener todos los sistemas de la IA
        if (TryGetComponent(out EnemyAIBase ai)) ai.enabled = false;
        if (TryGetComponent(out NavMeshAgent nav)) nav.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Die");

        // El objeto se destruye a los 4 segundos de morir
        Destroy(gameObject, 4f);
    }
}

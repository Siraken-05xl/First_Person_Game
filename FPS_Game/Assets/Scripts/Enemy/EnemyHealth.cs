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
        if (isDead) return;
        isDead = true;

        // AVISAR AL GAME MANAGER
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }

        // Desactivar IA
        if (TryGetComponent(out EnemyAIBase ai)) ai.enabled = false;
        if (TryGetComponent(out NavMeshAgent nav)) nav.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Die");

        Destroy(gameObject, 3f);
    }
}

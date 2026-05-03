using UnityEngine;
using UnityEngine.AI; // Necesario para detener el agente al morir

public class EnemyHealth : MonoBehaviour
{
    [Header("Health System Management")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int health;

    [Header("Feedback Configuration")]
    [SerializeField] Material damagedMat;
    [SerializeField] GameObject deathVfx;
    [SerializeField] MeshRenderer enemyRend;
    Material baseMat;

    // Referencias internas para la muerte
    private Animator anim;
    private NavMeshAgent agent;
    private EnemyAIBase aiScript;
    private bool isDead = false;

    private void Awake()
    {
        health = maxHealth;
        if (enemyRend != null) baseMat = enemyRend.material;

        // Obtenemos las referencias
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        aiScript = GetComponent<EnemyAIBase>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Si ya está muerto, no hace nada

        health -= damage;

        // Feedback visual de daño
        if (enemyRend != null && damagedMat != null)
        {
            enemyRend.material = damagedMat;
            Invoke(nameof(ResetEnemyMaterial), 0.1f);
        }

        // Comprobamos muerte aquí, no en el Update
        if (health <= 0)
        {
            Die();
        }
    }

    void ResetEnemyMaterial()
    {
        if (enemyRend != null) enemyRend.material = baseMat;
    }

    void Die()
    {
        isDead = true;
        health = 0;

        // 1. Activar animación de muerte de Mixamo
        if (anim != null) anim.SetTrigger("Die");

        // 2. Detener movimiento y disparos
        if (agent != null) agent.isStopped = true;
        if (aiScript != null) aiScript.enabled = false;

        // 3. Efectos de partículas
        if (deathVfx != null)
        {
            // Es mejor instanciarlo o moverlo, no solo activarlo si es hijo
            deathVfx.SetActive(true);
            deathVfx.transform.parent = null; // Para que no se destruya con el enemigo
        }

        Debug.Log("Enemigo eliminado");

        // 4. Opción: Desactivar el collider para que las balas lo atraviesen al estar en el suelo
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        // Destruir el objeto tras unos segundos para que de tiempo a ver la animación
        Destroy(gameObject, 4f);
    }
}

using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBase : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] Animator anim;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPoint;

    [Header("Combate")]
    [SerializeField] float timeBetweenAttacks = 0.8f;
    [SerializeField] float sightRange = 18f;
    [SerializeField] float attackRange = 8f;
    [SerializeField] float bulletSpeed = 40f;
    [SerializeField] int damagePerShot = 10;

    float nextAttackTime = 0f;

    private void Awake()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!enabled || target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > sightRange) Patroling();
        else if (dist <= sightRange && dist > attackRange) ChaseTarget();
        else AttackTarget();

        if (anim != null)
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    void Patroling() { if (agent.isOnNavMesh) agent.SetDestination(transform.position); }
    void ChaseTarget() { if (agent.isOnNavMesh) agent.SetDestination(target.position); }

    void AttackTarget()
    {
        if (agent.isOnNavMesh) agent.SetDestination(transform.position);

        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        if (Time.time >= nextAttackTime)
        {
            if (anim != null) anim.SetTrigger("isAttacking");
            Shoot();
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    void Shoot()
    {
        if (projectile == null || shootPoint == null) return;

        Vector3 direction = (target.position - shootPoint.position).normalized;
        GameObject bullet = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(direction));

        // LIMPIEZA: Se destruye en 1.2 segundos para que no se acumulen
        Destroy(bullet, 1.2f);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = direction * bulletSpeed;
        }

        // DAÑO: Le pegamos este pequeño "chip" a la bala para que detecte al jugador
        CollisionDetection cd = bullet.AddComponent<CollisionDetection>();
        cd.damage = damagePerShot;
    }
}

// Clase auxiliar para detectar el choque (va en el mismo archivo)
public class CollisionDetection : MonoBehaviour
{
    public int damage;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

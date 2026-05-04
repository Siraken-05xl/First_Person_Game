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
    [SerializeField] int damageToPlayer = 10;

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

        // ARREGLO DE ANIMACIONES
        if (anim != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            anim.SetBool("isWalking", isMoving);

            // Si el enemigo empieza a caminar o el jugador se aleja, deja de atacar
            if (isMoving || dist > attackRange)
            {
                anim.SetBool("isAttacking", false);
            }
        }
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
            if (anim != null) anim.SetBool("isAttacking", true);
            Shoot();
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    void Shoot()
    {
        if (projectile == null || shootPoint == null) return;

        Vector3 direction = (target.position - shootPoint.position).normalized;
        GameObject bullet = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(direction));

        // Limpieza rápida de balas del suelo
        Destroy(bullet, 1.2f);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = direction * bulletSpeed;
        }

        // Añadimos el componente de daño a la bala del enemigo
        CollisionDetection cd = bullet.AddComponent<CollisionDetection>();
        cd.damage = damageToPlayer;
        cd.targetTag = "Player";
    }
}

// Clase para detectar choques (va dentro del mismo archivo de EnemyAIBase)
public class CollisionDetection : MonoBehaviour
{
    public int damage;
    public string targetTag;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            if (targetTag == "Player")
            {
                PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(damage);
            }
            else if (targetTag == "Enemy")
            {
                // Buscamos el componente de salud en el enemigo o sus padres
                EnemyHealth eh = collision.gameObject.GetComponentInParent<EnemyHealth>();
                if (eh != null) eh.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}

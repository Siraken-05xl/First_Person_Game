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

    private bool alreadyAttacked;

    private void Awake()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
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

    void Patroling()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position);
    }

    void ChaseTarget()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(target.position);

        if (anim != null)
            anim.SetBool("isAttacking", false);
    }

    void AttackTarget()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position);

        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        if (!alreadyAttacked)
        {
            if (anim != null)
                anim.SetTrigger("Shoot"); // ?? IMPORTANTE

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // ?? ESTE MÉTODO LO LLAMA LA ANIMACIÓN
    public void Shoot()
    {
        Debug.Log("ENEMIGO DISPARA");

        if (projectile != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }
}

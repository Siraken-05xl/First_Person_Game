using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBase : MonoBehaviour
{
    #region General Variables
    [Header("AI Configuration")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Animator anim; // Referencia al Animator

    [Header("Patroling Stats")]
    [SerializeField] float walkPointRange = 8f;
    Vector3 walkPoint;
    bool walkPointSet;

    [Header("Attacking Stats")]
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootSpeedZ = 10f;
    bool alreadyAttacked;

    [Header("States & Detection Areas")]
    [SerializeField] float sightRange = 8f;
    [SerializeField] float attackRange = 5f; // Aumentado para que no se pegue tanto al disparar
    bool targetInSightRange;
    bool targetInAttackRange;

    [Header("Stuck Detection")]
    [SerializeField] float stuckCheckTime = 2f;
    [SerializeField] float stuckThreshold = 0.1f;
    [SerializeField] float maxStuckDuration = 3f;

    float stuckTimer;
    float lastCheckTime;
    Vector3 lastPosition;
    #endregion

    private void Awake()
    {
        // Si el player no se llama exactamente "Player", búscalo por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // Inicializamos el Animator

        lastPosition = transform.position;
        lastCheckTime = Time.time;
    }

    void Update()
    {
        EnemyStateUpdater();
        CheckIfStuck();
        UpdateAnimations(); // Nueva función para gestionar los booleanos del Animator
    }

    void EnemyStateUpdater()
    {
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, targetLayer);
        targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, targetLayer);

        if (!targetInSightRange && !targetInAttackRange) Patroling();
        if (targetInSightRange && !targetInAttackRange) ChaseTarget();
        if (targetInAttackRange && targetInSightRange) AttackTarget();
    }

    // --- LOGICA DE ANIMACIONES ---
    void UpdateAnimations()
    {
        // Si la velocidad del agente es mayor a un mínimo, está caminando
        // Usamos velocity.magnitude para que sea más preciso que el walkPointSet
        bool moving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("isWalking", moving);
    }

    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        if ((transform.position - walkPoint).sqrMagnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    void ChaseTarget()
    {
        agent.SetDestination(target.position);
    }

    void AttackTarget()
    {
        // Detenemos al agente para que dispare quieto
        agent.SetDestination(transform.position);

        // Mirar al objetivo
        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        if (!alreadyAttacked)
        {
            // ACTIVAR ANIMACION DE ATAQUE
            anim.SetBool("isAttacking", true);

            // Lógica de disparo
            Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(transform.forward)).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * shootSpeedZ, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
        anim.SetBool("isAttacking", false); // Volvemos a estado de caminar/idle
    }

    // El resto de funciones (CheckIfStuck, Gizmos) se mantienen igual...
    #region CheckStuck & Gizmos
    void CheckIfStuck()
    {
        if (Time.time - lastCheckTime > stuckCheckTime)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved < stuckThreshold && agent.hasPath) stuckTimer += stuckCheckTime;
            else stuckTimer = 0;

            if (stuckTimer >= maxStuckDuration)
            {
                walkPointSet = false;
                agent.ResetPath();
                stuckTimer = 0;
            }
            lastPosition = transform.position;
            lastCheckTime = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    #endregion
}

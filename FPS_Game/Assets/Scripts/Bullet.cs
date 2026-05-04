using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 20;
    public float speed = 40f;
    public float lifeTime = 2.5f;

    private float spawnTime;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        spawnTime = Time.time;

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - spawnTime < 0.05f) return;

        // 🔴 Daño a enemigos
        if (collision.gameObject.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
        }

        // 🔵 Daño al player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }
}

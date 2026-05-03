using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 20;
    public float lifeTime = 2.5f;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        // Limpieza automática para no saturar la memoria del juego
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // SEGURO: Ignora colisiones si ocurren en los primeros 0.05 segundos.
        // Esto evita que la bala choque con el cañón del arma al nacer.
        if (Time.time - spawnTime < 0.05f) return;

        // Intentar hacer daño si el objeto tiene salud
        if (collision.gameObject.TryGetComponent(out EnemyHealth health))
        {
            health.TakeDamage(damage);
        }

        // Se destruye al impactar con cualquier cosa sólida
        Destroy(gameObject);
    }
}

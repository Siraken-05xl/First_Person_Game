using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Camera fpsCam;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject projectilePrefab;

    [Header("Ajustes")]
    [SerializeField] float shootForce = 60f;
    [SerializeField] float shootingCooldown = 0.2f;
    [SerializeField] int damageToEnemy = 25;

    private bool canShoot = true;

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started && canShoot) StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, fpsCam.transform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(fpsCam.transform.forward * shootForce, ForceMode.VelocityChange);
            }

            // Inyectamos daño en la bala del jugador para que afecte al enemigo
            CollisionDetection cd = bullet.AddComponent<CollisionDetection>();
            cd.damage = damageToEnemy;
            cd.targetTag = "Enemy";

            // Destrucción de tus balas tras 2 segundos si no impactan
            Destroy(bullet, 2f);
        }
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Camera fpsCam;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject projectilePrefab;

    [Header("Ajustes de Arma")]
    [SerializeField] float shootForce = 60f;
    [SerializeField] float shootingCooldown = 0.15f;

    private bool canShoot = true;

    public void OnShoot(InputAction.CallbackContext context)
    {
        // Disparo instantáneo al presionar el botón (Input System)
        if (context.started && canShoot)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;
        ExecuteShot();
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }

    void ExecuteShot()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        // Instancia el proyectil siguiendo la orientación de la cámara
        GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, fpsCam.transform.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero; // Limpiar inercias previas

            // VelocityChange hace que la bala salga siempre a la misma velocidad sin importar su peso
            rb.AddForce(fpsCam.transform.forward * shootForce, ForceMode.VelocityChange);
        }
    }
}

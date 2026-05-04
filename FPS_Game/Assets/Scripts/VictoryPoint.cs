using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class VictoryPoint : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [SerializeField] float velocidadRotacion = 150f;
    [SerializeField] float amplitudFlote = 0.3f;
    [SerializeField] float velocidadFlote = 2.5f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // 1. ROTACIÓN CONSTANTE
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);

        // 2. EFECTO DE FLOTADO (Seno para movimiento suave)
        float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlote) * amplitudFlote;
        transform.position = new Vector3(posicionInicial.x, nuevoY, posicionInicial.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra tiene el Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Cargando escena de victoria!");

            // CAMBIO DE ESCENA DIRECTO
            SceneManager.LoadScene("SCN_Win");
        }
    }
}

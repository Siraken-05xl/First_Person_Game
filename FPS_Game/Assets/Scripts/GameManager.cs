using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Victoria")]
    public int enemiesToKill = 10;
    public int currentKills = 0;
    public GameObject victoryCube; // El cubo azul (flecha)

    private void Awake()
    {
        // Sistema Singleton para que sea fácil de llamar
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (victoryCube != null) victoryCube.SetActive(false);
    }

    public void AddKill()
    {
        currentKills++;
        Debug.Log("Enemigos derrotados: " + currentKills);

        if (currentKills >= enemiesToKill)
        {
            ShowVictoryPoint();
        }
    }

    void ShowVictoryPoint()
    {
        if (victoryCube != null)
        {
            victoryCube.SetActive(true);
            Debug.Log("¡Punto de victoria desbloqueado! Ve al cubo azul.");
        }
    }
}

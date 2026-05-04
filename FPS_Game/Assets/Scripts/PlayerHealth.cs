using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ajustes de Vida")]
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("¡Ay! Vida restante: " + health);

        if (health <= 0)
        {
            Debug.Log("Has muerto. Reiniciando...");
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        // Reinicia la escena en la que estás actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

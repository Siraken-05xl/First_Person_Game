using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    // Cambia "MenuPrincipal" por el nombre exacto de tu escena de menú
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("SCN_MainMenu");
    }
}

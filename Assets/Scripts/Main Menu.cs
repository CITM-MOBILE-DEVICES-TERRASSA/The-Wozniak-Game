using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Panel de ajustes que se asignará desde el Inspector

    public void PlayGame()
    {
        // Cargar la escena principal del juego
        SceneManager.LoadScene("Juego");
    }

    public void OpenSettings()
    {
        // Mostrar el panel de ajustes
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // Ocultar el panel de ajustes
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        // Salir del juego
        Application.Quit();
    }
}
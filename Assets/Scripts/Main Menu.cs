using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    private void Awake()
    {
        GameObject[] activePowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in activePowerUps)
        {
            Destroy(powerUp);
        }
        Debug.Log("Todos los Power-ups han sido destruidos.");
    }
    public void PlayGame()
    {
        PlayerPrefs.DeleteAll();    
        SceneManager.LoadScene("Juego");
    }

    public void ContinueGame()
    {
        Debug.Log("Botón 'Continuar' presionado. Cargando la escena...");

        SceneManager.LoadScene("Juego");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void TestLoadPlayerPrefs()
    {
        int vidas = PlayerPrefs.GetInt("Vidas", -1);
        int score = PlayerPrefs.GetInt("ActualScore", -1);
        int maxScore = PlayerPrefs.GetInt("00000", -1);
        int blockHits = PlayerPrefs.GetInt("BlockHits", -1);

        Debug.Log($"Vidas: {vidas}, Actual Score: {score}, Max Score: {maxScore}, Block Hits: {blockHits}");
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public GameObject settingsMenuUI;
    public GameObject UI;

    public Button Ajustes;
    public Button Automatic;

    public TrailRenderer ballTrailRenderer;
    public GameObject BouncyBall;

    public Button pauseButton;
    public TextMeshProUGUI countdownText;
    private bool isPaused = false;
    private float countdownTime = 3f;

    GameObject ball;
    GameObject paddle;

    public PlayerMovement playerMovement;

    void Start()
    {
        pauseButton.onClick.AddListener(OpenSettingsDirectly);
    }

    public void OpenSettingsDirectly()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            settingsMenuUI.SetActive(true);
            UI.SetActive(false);

            BouncyBall.GetComponent<SpriteRenderer>().enabled = false;
            ballTrailRenderer.enabled = false;

            Ajustes.gameObject.SetActive(false);
            Automatic.gameObject.SetActive(false);
        }
    }

    public void AutomaticGameplay()
    {
        if (playerMovement != null)
        {
            playerMovement.ToggleAutomaticGameplay();
            Debug.Log("Automatic gameplay toggled: ");
        }
    }

    public void SaveAndCloseSettings()
    {
        settingsMenuUI.SetActive(false);
        UI.SetActive(true);

        Automatic.gameObject.SetActive(true);
        Ajustes.gameObject.SetActive(true);
        BouncyBall.GetComponent<SpriteRenderer>().enabled = true;
        ballTrailRenderer.enabled = true;

        StartCoroutine(CountdownAndResume());
    }

    private IEnumerator CountdownAndResume()
    {
        countdownText.gameObject.SetActive(true);
        float countdown = countdownTime;

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString("F0");
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        countdownText.gameObject.SetActive(false);
        ResumeGame();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    public void ExitToMainMenu()
    {
        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
        levelGenerator.DestroyAllPowerUps();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    private List<GameObject> activePowerUps = new List<GameObject>();
    public GameObject[] powerUpPrefabs;

    public GameObject victoryPanel;

    public int blockHitsRequired = 1;

    private void Start()
    {
        StartCoroutine(RestoreGameState());
        GenerateLevel();
    }

    private IEnumerator RestoreGameState()
    {
        Debug.Log("Corutina 'RestoreGameState' iniciada.");

        yield return null; // Espera un frame

        int vidasGuardadas = PlayerPrefs.GetInt("Vidas", 3);
        Debug.Log("Vidas guardadas: " + vidasGuardadas);

        BouncyBall ball = FindObjectOfType<BouncyBall>();
        if (ball != null)
        {
            ball.lives = vidasGuardadas;
            Debug.Log("Vidas asignadas a la bola: " + ball.lives);

            // Actualizar las vidas visualmente
            for (int i = 0; i < ball.livesImage.Length; i++)
            {
                ball.livesImage[i].SetActive(i < ball.lives);
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto BouncyBall.");
        }

        // 2. Restaurar el puntaje actual
        int scoreGuardado = PlayerPrefs.GetInt("ActualScore", 0); // Valor por defecto es 0
        Debug.Log("Puntaje actual guardado: " + scoreGuardado);

        if (ball != null)
        {
            ball.score = scoreGuardado;
            ball.scoreTxt.text = ball.score.ToString("00000");
            Debug.Log("Puntaje actual asignado a la bola: " + ball.score);
        }

        // 3. Restaurar el puntaje máximo
        int maxScoreGuardado = PlayerPrefs.GetInt("00000", 0); // Valor por defecto es 0
        Debug.Log("Puntaje máximo guardado: " + maxScoreGuardado);

        if (ball != null)
        {
            ball.maxScore = maxScoreGuardado;
            ball.maxScoreTxt.text = ball.maxScore.ToString("00000");
            Debug.Log("Puntaje máximo asignado a la bola: " + ball.maxScore);
        }

        // 4. Restaurar los golpes necesarios para los bloques
        int golpesGuardados = PlayerPrefs.GetInt("BlockHits", 1); // Valor por defecto es 1
        Debug.Log("Golpes necesarios guardados: " + golpesGuardados);

        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null)
        {
            levelGenerator.blockHitsRequired = golpesGuardados;
            Debug.Log("Golpes necesarios asignados al generador: " + levelGenerator.blockHitsRequired);

            levelGenerator.GenerateLevel(); // Regenerar el nivel con los nuevos golpes necesarios
            Debug.Log("Nivel regenerado con " + levelGenerator.blockHitsRequired + " golpes necesarios.");
        }
        else
        {
            Debug.LogError("No se encontró el objeto LevelGenerator.");
        }
    }

    public void GenerateLevel()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = transform.position + new Vector3((float)((size.x - 1) * .5f - i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j / (size.y - 1));

                BlockComponent blockComponent = newBrick.AddComponent<BlockComponent>();
                blockComponent.hitsToBreak = blockHitsRequired;
                


                Debug.Log($"Bloque creado en {i}, {j} con {blockHitsRequired} golpes requeridos.");

                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.2f && powerUpPrefabs.Length > 0)
                {
                    GameObject powerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                    newBrick.AddComponent<PowerUpComponent>().powerUpPrefab = powerUp;
                }
            }
        }

        PlayerPrefs.SetInt("BlockHits", blockHitsRequired);
        PlayerPrefs.Save();
    }

    public void LevelCompleted()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        blockHitsRequired++;
        ClearLevel();

        DestroyAllPowerUps();

        PlayerPrefs.SetInt("BlockHits", blockHitsRequired);
        PlayerPrefs.SetInt("Vidas", FindObjectOfType<BouncyBall>().lives);
        PlayerPrefs.SetInt("ActualScore", FindObjectOfType<BouncyBall>().score);
        PlayerPrefs.SetInt("00000", FindObjectOfType<BouncyBall>().maxScore);
        PlayerPrefs.Save();

        GenerateLevel();
    }

    private void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
            DestroyAllPowerUps();
        }
    }

    public void Restart()
    {
        PowerUpComponent[] powerUpComponents = FindObjectsOfType<PowerUpComponent>();
        foreach (var component in powerUpComponents)
        {
            component.SetLevelResetting(true);
        }

        PlayerPrefs.DeleteKey("ActualScore");
        PlayerPrefs.DeleteKey("Vidas");
        PlayerPrefs.DeleteKey("BlockHits");
        PlayerPrefs.Save();
        DestroyAllPowerUps();

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DestroyAllPowerUps()
    {
        GameObject[] activePowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in activePowerUps)
        {
            Destroy(powerUp);
        }
        Debug.Log("Todos los Power-ups han sido destruidos.");  
    }
}